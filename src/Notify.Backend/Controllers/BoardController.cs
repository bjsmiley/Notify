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
using Notify.Backend.Application.Data;
using Notify.Backend.Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace Notify.Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BoardController : ControllerBase
	{
		private readonly IRabbitMQManager _rabbitMQManager;
		private readonly NotifyDBContext _context;
		private readonly IJwtService _jwtService;
		public BoardController(IRabbitMQManager rabbitMQManager, NotifyDBContext context, IJwtService jwtservice)
		{
			_rabbitMQManager = rabbitMQManager;
			_context = context;
			_jwtService = jwtservice;
			
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

			if (_context.Boards.Any(b => b.Name == command.Board))
				return BadRequest();

			var board = new Board
			{
				Name = command.Board,
				Key = command.Key,
				Created = DateTime.UtcNow
			};


			_context.Boards.Add(board);
			_context.SaveChanges();

			return Ok();
		}

		[Route("connect")]
		[HttpGet]
		public IActionResult Connect([FromBody] ConnectToBoardCommand command)
		{
			if (!_context.Boards.Any(b => b.Name == command.Board && b.Key == command.Key))
				return BadRequest();

			var token = _jwtService.GenerateToken(command.Board, command.Name);

			return Ok( new { token });
		}

		[Route("{name}/publish")]
		[HttpPost]
		[Authorize]
		public IActionResult Publish([FromRoute] string name, [FromBody] PublishMessageCommand command)
		{
			if (!command.Route.StartsWith(name)) return BadRequest();

			//_rabbitMQManager.Publish(command.Payload, name, ExchangeType.Topic, command.Route);

			return Ok(command.Payload);
		}
	}
}
