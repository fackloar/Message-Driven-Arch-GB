using MassTransit;
using Restaraunt.Messages;
using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Kitchen
{
    public class Manager
    {
        private readonly IBus _bus;

        public Manager(IBus bus)
        {
            _bus = bus;
        }

        public void CheckKitchenReady(Guid orderId, Dish? dish)
        {
            _bus.Publish<IKitchenReady>(new KitchenReady(orderId, true));
        }
    }
}
