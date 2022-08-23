using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Messaging
{
    public class Consumer
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IModel _channel;
        private readonly string _queueName;

        public Consumer(string queueName)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "sparrow-01.rmq.cloudamqp.com",
                VirtualHost = "rvapidqy",
                UserName = "rvapidqy", //Указать имя пользователя
                Password = "x1XJkf1mQU1iqkfCEfs1J7DeXhblPQkz", //Указать пароль
                Port = 5671,
                RequestedHeartbeat = TimeSpan.FromSeconds(10),
                Ssl = new SslOption
                {
                    Enabled = true,
                    AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                             SslPolicyErrors.RemoteCertificateChainErrors,
                    Version = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11
                }
            };

            _channel = _connectionFactory.CreateConnection().CreateModel();
            _queueName = queueName;
        }

        public void Receive(EventHandler<BasicDeliverEventArgs> receiveCallback)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += receiveCallback;

            _channel.BasicConsume(_queueName, true, consumer);
        }
    }
}
