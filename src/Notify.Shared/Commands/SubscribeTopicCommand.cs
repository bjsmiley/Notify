using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Shared.Commands
{
	public class SubscribeTopicCommand
	{
		//public string Callback { get; set; }
		public string Route { get; set; }
		public string Topic { get; set; }
	}
}
