using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Bus
{
	public interface IBus<T>
	{
		ValueTask SendAsync(T message);
		ValueTask<T> ReceiveAsync();
	}
}
