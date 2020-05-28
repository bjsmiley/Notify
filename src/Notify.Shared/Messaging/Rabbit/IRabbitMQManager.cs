using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace Notify.Shared.Messaging.Rabbit
{
    public interface IRabbitMQManager
    {
        void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey) where T : class;

        void Do(Action<IModel> action);
    }
}
