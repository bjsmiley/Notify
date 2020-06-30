using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Notify.Shared.Messaging.Rabbit
{
    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly IRabbitLifetimeConnection lifetimeConnection;

        public RabbitModelPooledObjectPolicy(IRabbitLifetimeConnection lifetimeConnection)
        {
            this.lifetimeConnection = lifetimeConnection;
        }

        

        public IModel Create()
        {
            return lifetimeConnection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                obj?.Dispose();
                return false;
            }
        }
    }
}
