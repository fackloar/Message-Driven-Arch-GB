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
        ILogger<BookingCancellationConsumer> _logger;

        public BookingCancellationConsumer(RestarauntClass restaraunt, IRepository<BookingCancellation> repository, ILogger<BookingCancellationConsumer> logger)
        {
            _restaraunt = restaraunt;
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IBookingCancellation> context)
        {
            if (context.Message.TableId is not { } tableId) return;

            var orderId = context.Message.OrderId;
            var messageId = context.MessageId.ToString();

            var model = _repository.Get().FirstOrDefault(i => i.OrderId == orderId);

            if (model is not null && model.CheckMessageId(messageId))
            {
                _logger.LogDebug($"Отмена второй раз {messageId}");
                return;
            }

            var requestModel = new BookingCancellation(context.Message, messageId);

            _logger.LogDebug($"Отмена первый раз {messageId}");
            var resultModel = model?.Update(requestModel, messageId) ?? requestModel;

            _repository.AddOrUpdate(resultModel);

            await _restaraunt.CancelBookingAsync(tableId);
            _logger.LogInformation($"[OrderId {context.Message.OrderId}] Отмена, столик {context.Message.TableId} освобожден.", context.Message.OrderId, tableId);
        }
    }
}
