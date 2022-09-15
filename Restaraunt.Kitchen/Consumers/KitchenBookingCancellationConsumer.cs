using MassTransit;
using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Kitchen.Consumers
{
    public class KitchenBookingCancellationConsumer : IConsumer<IBookingCancellation>
    {
        private readonly ILogger<KitchenBookingCancellationConsumer> _logger;

        public KitchenBookingCancellationConsumer(ILogger<KitchenBookingCancellationConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IBookingCancellation> context)
        {
            _logger.LogWarning($"[OrderId] {context.Message.OrderId}] Отмена на кухне!");
            return context.ConsumeCompleted;
        }
    }
}
