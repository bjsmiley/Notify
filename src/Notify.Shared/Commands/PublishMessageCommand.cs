using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Shared.Commands
{
	public class PublishMessageCommand
	{
		[Required]
		public string Route { get; set; }
		
		[Required]
		public object Payload { get; set; }
	}
}
