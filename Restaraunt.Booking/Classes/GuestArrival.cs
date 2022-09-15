namespace Restaraunt.Booking.Classes
{
    public class GuestArrival : IGuestArrival
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }

        public GuestArrival(RestarauntBooking instance)
        {
            OrderId = instance.OrderId;
            ClientId = instance.ClientId;
        }
    }
}
