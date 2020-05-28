using System;
using System.Collections.Generic;
using System.Text;

namespace Notify.Shared.Messaging.Rabbit
{
    public class RabbitMQSettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } = 5672;
    }
}
