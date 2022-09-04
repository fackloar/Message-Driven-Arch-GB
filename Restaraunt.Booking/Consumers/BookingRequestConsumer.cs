using MassTransit;
using Restaraunt.Messages.Interfaces;
using Restaraunt.Messages;
using Restaraunt.Booking.Classes;

namespace Restaraunt.Booking.Consumers
{
    public class BookingRequestConsumer : IConsumer<IBookingRequest>
    {
        private readonly RestarauntClass _restaurant;

        public BookingRequestConsumer(RestarauntClass restaurant)
        {
            _restaurant = restaurant;
        }

        public async Task Consume(ConsumeContext<IBookingRequest> context)
        {
            Console.WriteLine($"[OrderId: {context.Message.OrderId}]");
            var result = await _restaurant.BookFreeTableAsync(1);

            await context.Publish<ITableBooked>(new TableBooked(context.Message.OrderId, result ?? false));
        }
    }
}
