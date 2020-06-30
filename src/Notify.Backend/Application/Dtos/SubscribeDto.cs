using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Dtos
{
	public class SubscribeDto
	{
		public string ExchangeName { get; set; }
		public string QueueName { get; set; }
		public string RouteKey { get; set; }
		public string ConnectionId { get; set; }
	}
}
