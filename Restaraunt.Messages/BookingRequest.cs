using Restaraunt.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages
{
    public class BookingRequest : IBookingRequest
    {
        public BookingRequest(Guid orderId, Guid clientId, Dish? preOrder, DateTime creationDate, int estimatedTimeOfArrival)
        {
            OrderId = orderId;
            ClientId = clientId;
            PreOrder = preOrder;
            CreationDate = creationDate;
            EstimatedTimeOfArrival = estimatedTimeOfArrival;
        }

        public Guid OrderId { get; }
        public Guid ClientId { get; }
        public Dish? PreOrder { get; }

        public DateTime CreationDate { get; }
        public int EstimatedTimeOfArrival { get; }
    }
}
