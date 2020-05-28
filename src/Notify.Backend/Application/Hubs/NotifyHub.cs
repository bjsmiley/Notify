using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Notify.Backend.Application.Hubs
{
	public class NotifyHub : Hub
	{
		
		public override Task OnConnectedAsync()
		{
			this.Context.
			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			return base.OnDisconnectedAsync(exception);
		}


	}
}
