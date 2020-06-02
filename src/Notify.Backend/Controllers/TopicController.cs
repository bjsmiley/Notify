using Microsoft.AspNetCore.Mvc;
using Notify.Shared.Messaging.Rabbit;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notify.Backend.Application.Commands;

namespace Notify.Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TopicController : ControllerBase
	{
		private readonly IRabbitMQManager _rabbitMQManager;
		public TopicController(IRabbitMQManager rabbitMQManager)
		{
			_rabbitMQManager = rabbitMQManager;
		}

		//[Route("subscribe")]
		//[HttpPost]
		//public IActionResult Subscribe([FromBody] SubscribeTopicCommand command)
		//{

		//}
	}
}
