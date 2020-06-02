using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Models
{
	public class Board
	{
		[Key]
		public string Name { get; set; }
		public string Key { get; set; }
		public DateTime Created { get; set; }
	}
}
