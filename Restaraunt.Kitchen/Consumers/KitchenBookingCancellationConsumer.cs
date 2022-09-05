using MassTransit;
using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Kitchen.Consumers
{
    public class KitchenBookingCancellationConsumer : IConsumer<IBookingCancellation>
    {
        public Task Consume(ConsumeContext<IBookingCancellation> context)
        {
            Console.WriteLine($"[OrderId] {context.Message.OrderId}] Отмена на кухне!");
            return context.ConsumeCompleted;
        }
    }
}
