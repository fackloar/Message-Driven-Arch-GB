using MassTransit;
using Restaraunt.Booking.Classes;
using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Booking.Consumers
{
    public class BookingCancellationConsumer : IConsumer<IBookingCancellation>
    {
        private readonly RestarauntClass _restaraunt;

        public BookingCancellationConsumer(RestarauntClass restaraunt)
        {
            _restaraunt = restaraunt;
        }   

        public async Task Consume(ConsumeContext<IBookingCancellation> context)
        {
            if (context.Message.TableId is not { } tableId) return;
            await _restaraunt.CancelBookingAsync(tableId);
            Console.WriteLine($"[OrderId {context.Message.OrderId}] Отмена, столик {context.Message.TableId} освобожден.", context.Message.OrderId, tableId);
        }
    }
}
