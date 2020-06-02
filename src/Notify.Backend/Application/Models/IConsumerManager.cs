using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Models
{
	public interface IConsumerManager
	{
		public void AddConsumer(string routeKey);
	}
}
