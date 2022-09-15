using System.Collections.Concurrent;

namespace Restaraunt.Notification
{
    public class Notifier
    {
        private readonly ILogger<Notifier> _logger;

        public Notifier(ILogger<Notifier> logger)
        {
            _logger = logger;
        }

        public void Notify(Guid orderId, Guid clientId, string message)
        {
            _logger.LogInformation($"[OrderID: {orderId}] Уважаемый клиент {clientId}! {message}");
        }
    }
}
