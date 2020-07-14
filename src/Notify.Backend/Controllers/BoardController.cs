using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Notify.Shared.Commands;
using Notify.Shared.Models;
using Notify.Backend.Application.Data;
using Notify.Backend.Application.Models;


namespace Notify.Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BoardController : ControllerBase
	{
		private readonly NotifyDBContext _context;
		private readonly IJwtService _jwtService;
		public BoardController(NotifyDBContext context, IJwtService jwtservice)
		{
			_context = context;
			_jwtService = jwtservice;
			
		}

		[Route("create")]
		[HttpPost]
		public IActionResult Create([FromBody] CreateBoardCommand command)
		{

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

			return Ok( new TokenModel { Token = token });
		}

		
	}
}
