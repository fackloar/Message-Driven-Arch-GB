using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Messages
{
    public class TableBooked : ITableBooked
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
        public Dish? PreOrder { get; }
        public bool Success { get; }
        
        public TableBooked(Guid orderId, Guid clientId, bool success, Dish? preOrder = null)
        {
            OrderId = orderId;
            ClientId = clientId;
            PreOrder = preOrder;
            Success = success;
        }

    }
}