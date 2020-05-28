using Microsoft.AspNetCore.Mvc;
using Notify.Shared.Messaging.Rabbit;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
	}
}
