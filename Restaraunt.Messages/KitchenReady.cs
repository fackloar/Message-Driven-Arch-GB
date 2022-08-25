using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Messages
{
    public class KitchenReady : IKitchenReady
    {
        public Guid OrderId { get; }
        public bool isReady { get; }

        public KitchenReady(Guid orderId, bool isReady)
        {
            OrderId = orderId;
            this.isReady = isReady;
        }


    }
}