using MassTransit;
using Restaraunt.Messages;

namespace Restaraunt.Booking.Classes
{
    public class BookingWorker : BackgroundService
    {
        private const string _firstChoice = "1";
        private const string _secondChoice = "2";
        private readonly IBus _bus;
        private readonly RestarauntClass _restaraunt;

        public BookingWorker(IBus bus, RestarauntClass restaraunt)
        {
            _bus = bus;
            _restaraunt = restaraunt;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000, stoppingToken);

                Console.WriteLine(SpeakerBotLines.AskForNumberOfGuests);
                ushort numberOfGuests = ushort.Parse(Console.ReadLine());
                var result = await _restaraunt.BookFreeTableAsync(numberOfGuests);
                await _bus.Publish(new TableBooked(NewId.NextGuid(), NewId.NextGuid(), result ?? false),
                        context => context.Durable = false, stoppingToken);
            }
        }

    }

}
