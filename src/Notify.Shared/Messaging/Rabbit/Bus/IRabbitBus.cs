using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Shared.Messaging.Rabbit.Bus
{
	public interface IRabbitBus<T>
	{
		ValueTask SendAsync(T message);
		ValueTask<T> ReceiveAsync();
	}
}
