using Restaraunt.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages
{
    public class BookingCancellation : TransactionalData<BookingCancellation, IBookingCancellation>, IBookingCancellation
    {
        public BookingCancellation(Guid orderId, int? tableId)
        {
            OrderId = orderId;
            TableId = tableId;
        }

        public BookingCancellation(IBookingCancellation data, string messageId) : base(data, messageId)
        {

        }

        protected override void SetData(IBookingCancellation data)
        {
            OrderId = data.OrderId;
            TableId = data.TableId;
        }

        public Guid OrderId { get; private set; }
        public int? TableId { get; private set; }
    }
}
