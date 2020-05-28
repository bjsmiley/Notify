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
        private readonly RabbitMQSettings _options;

        private readonly IConnection _connection;

        public RabbitModelPooledObjectPolicy(IOptions<RabbitMQSettings> optionsAccs)
        {
            _options = optionsAccs.Value;
            _connection = GetConnection();
        }

        private IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _options.Host,
                UserName = _options.UserName,
                Password = _options.Password,
                Port = _options.Port
            };

            var attempts = 0;

            while (true)
            {
                try
                {
                    return factory.CreateConnection();
                }
                catch (Exception e)
                {
                    if (attempts >= 5) throw e;

                    attempts++;
                    Task.Delay(1000).Wait();
                }
            }

        }

        public IModel Create()
        {
            return _connection.CreateModel();
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
