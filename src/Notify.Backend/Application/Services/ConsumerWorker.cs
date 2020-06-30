using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Notify.Backend.Application.Hubs;
using Notify.Shared.Messaging.Rabbit.Bus;
using Notify.Shared.Messaging.Rabbit.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Services
{
	public class ConsumerWorker : BackgroundService
	{
		private readonly DefaultObjectPool<IModel> channelPool;
		private readonly IRabbitBus<SubscribeDto> bus;
		private readonly ILogger<ConsumerWorker> logger;
		private readonly IHubContext<NotifyHub> hubContext;
		private IModel mainChannel;

		public ConsumerWorker(IPooledObjectPolicy<IModel> channelPolicy, IRabbitBus<SubscribeDto> bus, ILogger<ConsumerWorker> logger, IHubContext<NotifyHub> hubContext)
		{
			this.channelPool = new DefaultObjectPool<IModel>(channelPolicy, Environment.ProcessorCount * 2);
			this.bus = bus;
			this.logger = logger;
			this.hubContext = hubContext;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			mainChannel = channelPool.Get();

			while (!stoppingToken.IsCancellationRequested)
			{
				Consume(await bus.ReceiveAsync());
			}

			mainChannel.Close();
			channelPool.Return(mainChannel);
		}

		private void Consume(SubscribeDto newSubscription)
		{
			mainChannel.ExchangeDeclare(newSubscription.ExchangeName, ExchangeType.Topic, true, false, null);

			mainChannel.QueueDeclare(newSubscription.QueueName, true, false, true);
			mainChannel.QueueBind(newSubscription.QueueName, newSubscription.ExchangeName, newSubscription.RouteKey);

			var consumer = new EventingBasicConsumer(mainChannel);

			consumer.Received += async (sender, args) =>
			{
				//logger.LogInformation("received");

				var responseObject = JsonSerializer.Deserialize(args.Body.Span, typeof(object));
				await hubContext.Clients.Client(newSubscription.ConnectionId).SendAsync(newSubscription.QueueName, responseObject);
				
				mainChannel.BasicAck(args.DeliveryTag, false);
			};



			mainChannel.BasicConsume(newSubscription.QueueName, false, consumer);

			logger.LogInformation($"Connection [{newSubscription.ConnectionId}] - Subscribing to  queue '{newSubscription.QueueName}' on route '{newSubscription.RouteKey}'");
		}
	}
}
