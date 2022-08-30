using System;
using System.Threading.Tasks;
using MassTransit;
using Restaraunt.Messages.Interfaces;
using Restaraunt.Notification;

namespace Restaurant.Notification.Consumers
{
    public class NotifierTableBookedConsumer : IConsumer<ITableBooked>
    {
        private readonly Notifier _notifier;

        public NotifierTableBookedConsumer(Notifier notifier)
        {
            _notifier = notifier;
        }

        public  Task Consume(ConsumeContext<ITableBooked> context)
        {
           var result = context.Message.Success;

           _notifier.Accept(context.Message.OrderId, result ? Accepted.Booking : Accepted.Rejected,
               context.Message.ClientId);

           return Task.CompletedTask;
        }
    }
}