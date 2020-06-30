using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System.Text.Json;
using RabbitMQ.Client.Events;

namespace Notify.Shared.Messaging.Rabbit
{
    public class RabbitMQManager : IRabbitMQManager
    {
        private readonly DefaultObjectPool<IModel> _objectPool;

        public RabbitMQManager(IPooledObjectPolicy<IModel> objectPolicy)
        {
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
        }

        public void Do(Action<IModel> action)
        {
            var channel = _objectPool.Get();

            try
            {
                action(channel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _objectPool.Return(channel);
            }

        }

        public void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey)
            where T : class
        {
            if (message == null)
                return;

            var channel = _objectPool.Get();
            try
            {
                channel.ExchangeDeclare(exchangeName, exchangeType, true, false, null);

                var sendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }

        public void Subscribe(string exchangeName, string queueName, string routeKey, EventHandler<BasicDeliverEventArgs> eventHandler)
        {
            var channel = _objectPool.Get();


            try
            {
                channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, true, false, null);

                channel.QueueDeclare(queueName, true, false, true);
                channel.QueueBind(queueName, exchangeName, routeKey);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += eventHandler;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }

    }
}
