using MassTransit;
using Restaraunt.Messages.Interfaces;
using Restaraunt.Messages;
using Restaraunt.Booking.Classes;

namespace Restaraunt.Booking.Consumers
{
    public class BookingRequestConsumer : IConsumer<IBookingRequest>
    {
        private readonly RestarauntClass _restaurant;
        private readonly IRepository<BookingRequest> _repository;

        public BookingRequestConsumer(RestarauntClass restaurant, IRepository<BookingRequest> repository)
        {
            _restaurant = restaurant;
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<IBookingRequest> context)
        {
            var orderId = context.Message.OrderId;
            var messageId = context.MessageId.ToString();

            var model = _repository.Get().FirstOrDefault(i => i.OrderId == orderId);

            if (model is not null && model.CheckMessageId(messageId))
            {
                Console.WriteLine($"Запрос брони второй раз {messageId}");
                return;
            }

            var requestModel = new BookingRequest(context.Message, messageId);

            Console.WriteLine($"Запрос брони первый раз {messageId}");
            var resultModel = model?.Update(requestModel, messageId) ?? requestModel;

            _repository.AddOrUpdate(resultModel);

            Console.WriteLine($"OrderId: {orderId}");
            var (success, tableId) = await _restaurant.BookFreeTableAsync(1);

            await context.Publish<ITableBooked>(new TableBooked(orderId, tableId!.Value));
        }
    }
}
