namespace Restaraunt.Booking.Classes
{
    public interface IGuestArrival
    {
        Guid OrderId { get; }
        Guid ClientId { get; }
    }
}
