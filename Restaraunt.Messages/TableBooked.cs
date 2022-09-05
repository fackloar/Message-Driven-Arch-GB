using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Messages
{
    public class TableBooked : ITableBooked
    {
        public Guid OrderId { get; }
        public int TableId { get; }
        public DateTime CreationDate { get; }
        
        public TableBooked(Guid orderId, int tableId)
        {
            OrderId = orderId;
            TableId = tableId;
        }

    }
}