using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Notify.Backend.Application.Bus
{
	public class Bus<T> : IBus<T>
	{
		private readonly Channel<T> innerChannel;
		public Bus()
		{
			innerChannel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
			{
				SingleReader = true,
				SingleWriter = true
			});
		}

		public ValueTask<T> ReceiveAsync()
		{
			return innerChannel.Reader.ReadAsync();
		}

		public ValueTask SendAsync(T message)
		{
			return innerChannel.Writer.WriteAsync(message);
		}
	}
}
