using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notify.Shared.Messaging.Rabbit
{
	public interface IRabbitLifetimeConnection : IDisposable
	{
		public IModel CreateModel();
	}
}
