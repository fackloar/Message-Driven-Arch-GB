using Messaging;
using System.Text;

namespace Restaraunt.Notification
{
    public class Worker : BackgroundService
    {
        private readonly Consumer _consumer;

        public Worker()
        {
            _consumer = new Consumer("Orders");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Receive((sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("[x] Received {0}", message);
            });
        }
    }
}
