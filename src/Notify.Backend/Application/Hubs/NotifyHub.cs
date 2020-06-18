using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Notify.Backend.Application.Commands;
using Notify.Backend.Application.Models;
using Notify.Shared.Messaging.Rabbit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notify.Backend.Application.Hubs
{
	[Authorize]
	public class NotifyHub : Hub
	{
		private readonly IRabbitMQManager rabbitManager;

		public NotifyHub(IRabbitMQManager rabbitManager)
		{
			this.rabbitManager = rabbitManager;
		}
		
		//public override Task OnConnectedAsync()
		//{
		//	return base.OnConnectedAsync();
		//}

		//public override Task OnDisconnectedAsync(Exception exception)
		//{
		//	return base.OnDisconnectedAsync(exception);
		//}

		//public Task Subscribe(string boardName, string topic, string routeKey, string callback)
		//{
		//	EventHandler<BasicDeliverEventArgs> eventHandler = async (sender, args) =>
		//	{
		//		var responseObject = JsonSerializer.Deserialize(args.Body.Span, typeof(object));

		//		var id = Context.ConnectionId;

		//		await Clients.Client(id).SendAsync(callback, responseObject);
		//	};

		//	rabbitManager.Subscribe(boardName, topic, routeKey, eventHandler);
			

		//	return Task.CompletedTask;
		//}

		public Task Subscribe(SubscribeTopicCommand command)
		{
			var user = Context.User.FindFirst(ClaimTypes.Name)?.Value;
			var board = Context.User.FindFirst(ClaimTypes.GroupSid)?.Value;

			if (command.Route.StartsWith(board))
			{
				EventHandler<BasicDeliverEventArgs> eventHandler = async (sender, args) =>
				{
					var responseObject = JsonSerializer.Deserialize(args.Body.Span, typeof(object));

					var id = Context.ConnectionId;

					await Clients.Client(id).SendAsync(command.Callback, responseObject);
				};

				rabbitManager.Subscribe(board, command.Topic, command.Route, eventHandler);
			}
			else
			{
				this.Context.Abort();
			}

			return Task.CompletedTask;
		}

		public Task Publish(PublishMessageCommand command)
		{
			var board = Context.User.FindFirst(ClaimTypes.GroupSid)?.Value;

			if (command.Route.StartsWith(board))
			{
				rabbitManager.Publish(command.Payload, board, ExchangeType.Topic, command.Route);
			}
			else
			{
				this.Context.Abort();
			}

			return Task.CompletedTask;
		}




	}
}
