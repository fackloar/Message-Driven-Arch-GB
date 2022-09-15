using MassTransit;
using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Booking.Consumers
{
    public class BookingRequestFaultConsumer : IConsumer<Fault<IBookingRequest>>
    {
        private readonly ILogger<BookingRequestFaultConsumer> _logger;

        BookingRequestFaultConsumer(ILogger<BookingRequestFaultConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Fault<IBookingRequest>> context)
        {
            _logger.LogWarning($"[OrderId {context.Message.Message.OrderId}] Отмена в зале");
            return Task.CompletedTask;
        }
    }
}
