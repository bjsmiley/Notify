using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Notify.Backend.Application.Bus;
using Notify.Backend.Application.Dtos;
using Notify.Backend.Application.Hubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Services
{
	public class ConsumerWorker : BackgroundService
	{
		private readonly DefaultObjectPool<IModel> channelPool;
		private readonly IBus<SubscribeDto> bus;
		private readonly ILogger<ConsumerWorker> logger;
		private readonly IHubContext<NotifyHub> hubContext;
		private IModel mainChannel;
		private List<string> queueNames = new List<string>();

		public ConsumerWorker(IPooledObjectPolicy<IModel> channelPolicy, IBus<SubscribeDto> bus, ILogger<ConsumerWorker> logger, IHubContext<NotifyHub> hubContext)
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
				consume(await bus.ReceiveAsync());
			}

			checkConsumers();

			mainChannel.Close();
			channelPool.Return(mainChannel);
		}

		private void consume(SubscribeDto newSubscription)
		{
			mainChannel.ExchangeDeclare(newSubscription.ExchangeName, ExchangeType.Topic, true, false, null);

			mainChannel.QueueDeclare(newSubscription.QueueName, true, false, true);
			mainChannel.QueueBind(newSubscription.QueueName, newSubscription.ExchangeName, newSubscription.RouteKey);

			queueNames.Add(newSubscription.QueueName);
			
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

		private void checkConsumers()
		{
			foreach(var queue in queueNames)
			{
				try
				{
					if (mainChannel.ConsumerCount(queue) > 0)
					{
						logger.LogError("Closing main consumer channel even though queue '{queue}' still has consumers.",queue);
					}
				}
				catch (Exception e) 
				{
					logger.LogError("Checking consumer count of queue '{queue}' led to Exception: {e}", queue, e);
				}
				
			}
		}
	}
}
