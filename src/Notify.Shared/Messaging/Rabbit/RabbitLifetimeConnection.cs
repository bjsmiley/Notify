using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Shared.Messaging.Rabbit
{
	public class RabbitLifetimeConnection : IRabbitLifetimeConnection
	{
		private readonly IConnection _connection;
        private readonly RabbitMQSettings _options;
        public RabbitLifetimeConnection(IOptions<RabbitMQSettings> optionsAccs)
		{
            _options = optionsAccs.Value;
            _connection = GetConnection();
		}

        public IModel CreateModel() => _connection.CreateModel();

        private IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                //HostName = _options.Host,
                //UserName = _options.UserName,
                //Password = _options.Password,
                //Port = _options.Port
                Uri = new Uri(_options.ConnectionString),
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

        public void Dispose() => _connection.Close();
    }
}
