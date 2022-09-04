using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Messages
{
    public class TableBooked : ITableBooked
    {
        public Guid OrderId { get; }
        public bool Success { get; }
        public DateTime CreationDate { get; }
        
        public TableBooked(Guid orderId, bool success)
        {
            OrderId = orderId;
            Success = success;
        }

    }
}