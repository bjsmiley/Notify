using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Commands
{
	public class CreateBoardCommand
	{
		[Required(AllowEmptyStrings =false)]
		[MinLength(5)]
		public string Name { get; set; }

		[MinLength(5)]
		public string Key { get; set; }
	}
}
