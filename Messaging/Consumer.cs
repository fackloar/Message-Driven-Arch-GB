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
        private readonly Dictionary<string, object> _arguments;

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
            _arguments = new Dictionary<string, object>();
            _arguments.Add("rvapidqy-max-length", 10000);
        }

        public void Receive(EventHandler<BasicDeliverEventArgs> receiveCallback)
        {
            _channel.ExchangeDeclare(
                exchange: "direct_exchange",
                type: "direct",
                durable: true);

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: _arguments);

            _channel.QueueBind(
                queue: _queueName,
                exchange: "direct_exchange",
                routingKey: "black");

            
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += receiveCallback;

            _channel.BasicConsume(_queueName, true, consumer);
        }
    }
}
