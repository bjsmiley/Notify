using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Notify.Shared.Messaging.Rabbit.Bus
{
	public class RabbitBus<T> : IRabbitBus<T>
	{
		private readonly Channel<T> innerChannel;
		public RabbitBus()
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
