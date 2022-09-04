using MassTransit;
using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Kitchen.Consumers
{
    public class KitchenBookingRequestFaultConsumer : IConsumer<Fault<IBookingRequest>>
    {
        public Task Consume(ConsumeContext<Fault<IBookingRequest>> context)
        {
            return Task.CompletedTask;
        }
    }
}
