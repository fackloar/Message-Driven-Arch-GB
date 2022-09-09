using Restaraunt.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages
{
    public sealed class BookingRequest : TransactionalData<BookingRequest, IBookingRequest>, IBookingRequest
    {
        public BookingRequest(Guid orderId, Guid clientId, Dish? preOrder, DateTime creationDate, int estimatedTimeOfArrival)
        {
            OrderId = orderId;
            ClientId = clientId;
            PreOrder = preOrder;
            CreationDate = creationDate;
            EstimatedTimeOfArrival = estimatedTimeOfArrival;
        }

        public BookingRequest(IBookingRequest model, string messageId) : base(model, messageId)
        {

        }

        protected override void SetData(IBookingRequest data)
        {
            OrderId = data.OrderId;
            ClientId = data.ClientId;
            PreOrder = data.PreOrder;
            CreationDate = data.CreationDate;
            EstimatedTimeOfArrival = data.EstimatedTimeOfArrival;
        }

        public Guid OrderId { get; private set; }
        public Guid ClientId { get; private set; }
        public Dish? PreOrder { get; private set; }

        public DateTime CreationDate { get; private set; }
        public int EstimatedTimeOfArrival { get; private set; }
    }
}
