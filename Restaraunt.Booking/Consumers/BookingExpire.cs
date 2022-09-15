using Restaraunt.Booking.Classes;

namespace Restaraunt.Booking.Consumers
{
    public class BookingExpire : IBookingExpire
    {
        private readonly RestarauntBooking _instance;

        public BookingExpire(RestarauntBooking instance)
        {
            _instance = instance;
        }

        public Guid OrderId => _instance.OrderId;
    }
}
