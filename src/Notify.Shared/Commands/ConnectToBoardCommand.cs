using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Shared.Commands
{
	public class ConnectToBoardCommand
	{
		public string Board { get; set; }
		public string Name { get; set; }
		public string Key { get; set; }
	}
}
