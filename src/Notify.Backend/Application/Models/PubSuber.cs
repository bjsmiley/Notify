using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Models
{
	public class PubSuber
	{
		public string Name { get; set; }
		public string Board { get; set; }
		public List<string> Topics { get; set; }

	}
}
