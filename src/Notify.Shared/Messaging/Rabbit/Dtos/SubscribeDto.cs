using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notify.Shared.Messaging.Rabbit.Dtos
{
	public class SubscribeDto
	{
		public string ExchangeName { get; set; }
		public string QueueName { get; set; }
		public string RouteKey { get; set; }
		public string ConnectionId { get; set; }
}
}
