using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notify.Shared.Messaging.Rabbit;
using Notify.Backend.Application.Commands;
using RabbitMQ.Client;
using Microsoft.AspNetCore.SignalR;
using Notify.Backend.Application.Hubs;

namespace Notify.Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BoardController : ControllerBase
	{
		private readonly IRabbitMQManager _rabbitMQManager;
		public BoardController(IRabbitMQManager rabbitMQManager)
		{
			_rabbitMQManager = rabbitMQManager;
			
		}

		[Route("create")]
		[HttpPost]
		public IActionResult Create([FromBody] CreateBoardCommand command)
		{
			//_rabbitMQManager.Do(connection =>
			//{
			//	connection.ExchangeDeclare(command.Name, ExchangeType.Topic, true, false, null);
			//});

			// Simply validate that the command.Name does not already exist and return a jwt with important
			// information:
			// exchange name
			// expire time?
			// what about the user id?

			return Ok();
		}

		[Route("{name}/publish")]
		[HttpPost]
		public IActionResult Publish([FromRoute] string name, [FromBody] PublishMessageCommand command)
		{
			if (!command.Route.StartsWith(name)) return BadRequest();

			_rabbitMQManager.Publish(command.Payload, name, ExchangeType.Topic, command.Route);

			return Ok();
		}
	}
}
