using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Notify.Backend.Application.Models;
using Notify.Shared.Messaging.Rabbit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notify.Backend.Application.Hubs
{
	public class NotifyHub : Hub
	{
		private readonly IRabbitMQManager rabbitManager;

		public NotifyHub(IRabbitMQManager rabbitManager)
		{
			this.rabbitManager = rabbitManager;
		}
		
		public override Task OnConnectedAsync()
		{
			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			return base.OnDisconnectedAsync(exception);
		}

		public Task Subscribe(string boardName, string topic, string routeKey, string callback)
		{
			EventHandler<BasicDeliverEventArgs> eventHandler = (sender, args) =>
			{
				var responseObject = JsonSerializer.Deserialize(args.Body.Span, typeof(object));

				var id = Context.ConnectionId;

				Clients.Client(id).SendAsync(callback, responseObject);
			};

			rabbitManager.Subscribe(boardName, topic, routeKey, eventHandler);
			

			return Task.CompletedTask;
		}




	}
}
