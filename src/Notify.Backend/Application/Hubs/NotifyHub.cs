﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Notify.Backend.Application.Commands;
using Notify.Shared.Messaging.Rabbit;
using Notify.Shared.Messaging.Rabbit.Bus;
using Notify.Shared.Messaging.Rabbit.Dtos;
using RabbitMQ.Client;

namespace Notify.Backend.Application.Hubs
{
	[Authorize]
	public class NotifyHub : Hub
	{
		private readonly IRabbitMQManager rabbitManager;
		private readonly IRabbitBus<SubscribeDto> bus;
		private readonly ILogger<NotifyHub> logger;

		public NotifyHub(IRabbitMQManager rabbitManager, IRabbitBus<SubscribeDto> bus, ILogger<NotifyHub> logger)
		{
			this.rabbitManager = rabbitManager;
			this.bus = bus;
			this.logger = logger;
		}

		public async Task Subscribe(SubscribeTopicCommand command)
		{
			var user = Context.User.FindFirst(ClaimTypes.Name)?.Value;
			var board = Context.User.FindFirst(ClaimTypes.GroupSid)?.Value;

			if (command.Route.StartsWith(board))
			{
				await bus.SendAsync(new SubscribeDto
				{
					ExchangeName = board,
					QueueName = command.Topic,
					RouteKey = command.Route,
					ConnectionId = Context.ConnectionId
				});				
			}
			else
			{
				this.Context.Abort();
			}
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
