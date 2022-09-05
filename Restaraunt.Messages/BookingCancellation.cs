using Restaraunt.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages
{
    public class BookingCancellation : IBookingCancellation
    {
        public BookingCancellation(Guid orderId, int? tableId)
        {
            OrderId = orderId;
            TableId = tableId;
        }

        public Guid OrderId { get; }
        public int? TableId { get; }
    }
}
