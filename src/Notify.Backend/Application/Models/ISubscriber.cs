using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Models
{
	public interface ISubscriber
	{
		Task Subscribe(string boardName, string topic, string routeKey, string callback);
	}
}
