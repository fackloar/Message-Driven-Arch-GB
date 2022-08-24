using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Messaging
{
    public class Producer
    {
        private readonly ConnectionFactory _connectionFactory;

        public Producer()
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
        }

        private void SendToQueue(byte[] data, string queueName)
        {
            if (data.Length > 0)
            {
                try
                {
                    using var connection = _connectionFactory.CreateConnection();
                    using var channel = connection.CreateModel();

                    channel.ExchangeDeclare(
                        exchange: "direct_exchange",
                        type: "direct",
                        durable: true);

                    channel.BasicPublish(
                        exchange: "direct_exchange",
                        routingKey: "black",
                        body: data);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка отправки сообщения в очередь {queueName}. " +
                                      $"Сведения о сообщении: '{Encoding.UTF8.GetString(data, 0, data.Length)}' " +
                                      $"Сведения об ошибке:" + e.Message + "/" + e?.InnerException);
                }
            }
        }

        public void Send(string message)
        {
            var queueName = "Orders";
            var byteArr = Encoding.UTF8.GetBytes(message);
            SendToQueue(byteArr, queueName);
        }
    }
}
