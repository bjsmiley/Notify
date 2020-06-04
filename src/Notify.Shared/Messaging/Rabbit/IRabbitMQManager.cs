using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notify.Shared.Messaging.Rabbit
{
    public interface IRabbitMQManager
    {
        void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey) where T : class;

        void Subscribe(string exchangeName, string queueName, string routeKey, EventHandler<BasicDeliverEventArgs> eventHandler);

        void Do(Action<IModel> action);
    }
}
