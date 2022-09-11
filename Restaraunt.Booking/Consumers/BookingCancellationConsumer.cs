using MassTransit;
using Restaraunt.Booking.Classes;
using Restaraunt.Messages;
using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Booking.Consumers
{
    public class BookingCancellationConsumer : IConsumer<IBookingCancellation>
    {
        private readonly RestarauntClass _restaraunt;
        private readonly IRepository<BookingCancellation> _repository;

        public BookingCancellationConsumer(RestarauntClass restaraunt, IRepository<BookingCancellation> repository)
        {
            _restaraunt = restaraunt;
            _repository = repository;
        }   

        public async Task Consume(ConsumeContext<IBookingCancellation> context)
        {
            if (context.Message.TableId is not { } tableId) return;

            var orderId = context.Message.OrderId;
            var messageId = context.MessageId.ToString();

            var model = _repository.Get().FirstOrDefault(i => i.OrderId == orderId);

            if (model is not null && model.CheckMessageId(messageId))
            {
                Console.WriteLine($"Отмена второй раз {messageId}");
                return;
            }

            var requestModel = new BookingCancellation(context.Message, messageId);

            Console.WriteLine($"Отмена первый раз {messageId}");
            var resultModel = model?.Update(requestModel, messageId) ?? requestModel;

            _repository.AddOrUpdate(resultModel);

            await _restaraunt.CancelBookingAsync(tableId);
            Console.WriteLine($"[OrderId {context.Message.OrderId}] Отмена, столик {context.Message.TableId} освобожден.", context.Message.OrderId, tableId);
        }
    }
}
