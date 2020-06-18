using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Commands
{
	public class CreateBoardCommand
	{
		public string Board { get; set; }
		public string Key { get; set; }
	}
}
