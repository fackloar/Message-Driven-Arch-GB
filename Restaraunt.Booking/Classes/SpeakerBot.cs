using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Messaging;

namespace Restaraunt.Booking.Classes
{
    /// <summary>
    /// класс для взаимодействия с пользователем
    /// </summary>
    internal class SpeakerBot
    {
        private readonly RestarauntClass _restaraunt;
        private readonly TimeSpan interval = TimeSpan.FromMinutes(1);
        private const string _firstChoice = "1";
        private const string _secondChoice = "2";
        private readonly IBus _bus;
        public SpeakerBot(RestarauntClass restaraunt, IBus bus)
        {
            _restaraunt = restaraunt;
            _bus = bus;

            CancelBookingTimer(interval);
        }
        /// <summary>
        /// Метод для запуска взаимодействия с пользователем
        /// </summary>
        public void InitialHello()
        {
            while (true)
            {
                Console.WriteLine(SpeakerBotLines.InitialHelloChoice);

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                switch (Console.ReadLine())
                {
                    case _firstChoice:
                        ChooseTypeOfBooking();
                        break;
                    case _secondChoice:
                        ChooseTypeOfCancellation();
                        break;
                    default:
                        Console.WriteLine(SpeakerBotLines.Input1Or2);
                        break;
                }

                stopWatch.Stop();
                var ts = stopWatch.Elapsed;
                Console.WriteLine($"{ts.Seconds:00}:{ts.Milliseconds:00}");
            }
        }

        private async void ChooseTypeOfBooking()
        {
            Console.WriteLine(SpeakerBotLines.BookingChoice);
            bool validInput = false;
            while (!validInput)
            {
                switch (Console.ReadLine())
                {
                    case _firstChoice:
                        validInput = true;
                        Console.WriteLine(SpeakerBotLines.WaitForBooking);
                        Console.WriteLine(SpeakerBotLines.YoullBeNotified);
                        var table = await _restaraunt.BookFreeTableAsync(1);
                        NotifyOnBooking(table);
                        break;
                    case _secondChoice:
                        validInput = true;
                        Console.WriteLine(SpeakerBotLines.WaitForBooking);
                        Console.WriteLine(SpeakerBotLines.StayOnLine);
                        table = _restaraunt.BookFreeTable(1);
                        _producer.Send(table is null
                            ? SpeakerBotLines.AllTablesOccupied
                            : SpeakerBotLines.BookingReady + table.Id);
                        break;
                    default:
                        Console.WriteLine(SpeakerBotLines.Input1Or2);
                        break;
                }
            }
        }

        private async void ChooseTypeOfCancellation()
        {
            Console.WriteLine(SpeakerBotLines.CancellationChoice);
            bool validInput = false;
            while (!validInput)
            {
                switch (Console.ReadLine())
                {
                    case _firstChoice:
                        validInput = true;
                        CancelAsync();
                        break;
                    case _secondChoice:
                        validInput = true;
                        Cancel();
                        break;
                    default:
                        Console.WriteLine(SpeakerBotLines.Input1Or2);
                        break;
                }
            }
        }

        private async void CancelAsync()
        {
            bool validInput = false;
            while (!validInput)
            {
                Console.WriteLine(SpeakerBotLines.WaitForCancellation);
                if (!int.TryParse(Console.ReadLine(), out var tableId) || tableId < 0 || tableId > 10)
                {
                    Console.WriteLine(SpeakerBotLines.InputTableId);
                }
                else if (!_restaraunt.CheckIfBooked(tableId))
                {
                    validInput = true;
                    _producer.Send(tableId + SpeakerBotLines.TableNotOccupied);
                }
                else
                {
                    validInput = true;
                    Console.WriteLine(SpeakerBotLines.YoullBeNotified);
                    var table = await _restaraunt.CancelBookingAsync(tableId);
                    NotifyOnCancellation(table);
                }
            }
        }

        private void Cancel()
        {
            bool validInput = false;
            while (!validInput)
            {
                Console.WriteLine(SpeakerBotLines.WaitForCancellation);
                if (!int.TryParse(Console.ReadLine(), out var tableId) || tableId < 0 || tableId > 10)
                {
                    Console.WriteLine(SpeakerBotLines.InputTableId);
                }
                else if (!_restaraunt.CheckIfBooked(tableId))
                {
                    validInput = true;
                    _producer.Send(tableId + SpeakerBotLines.TableNotOccupied);
                }
                else
                {
                    validInput = true;
                    Console.WriteLine(SpeakerBotLines.StayOnLine);
                    var table = _restaraunt.CancelBooking(tableId);
                    _producer.Send(SpeakerBotLines.CancellationReady + table.Id);
                }
            }
        }

        private async void NotifyOnBooking(Table table)
        {
            await Task.Run(async () =>
            {
                await Task.Delay(1000);
                _producer.Send(table is null
                    ? SpeakerBotLines.Notification + SpeakerBotLines.AllTablesOccupied
                    : SpeakerBotLines.Notification + SpeakerBotLines.BookingReady + table.Id);
            });
        }
        private async void NotifyOnCancellation(Table table)
        {
            await Task.Run(async () =>
            {
                await Task.Delay(1000);
                _producer.Send(SpeakerBotLines.Notification + SpeakerBotLines.CancellationReady + table.Id);
            });
        }
        /// <summary>
        /// автоматически отменет бронь стола спустя какое-то время
        /// </summary>
        /// <param name="interval">интервал отмены брони</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task CancelBookingTimer(TimeSpan interval, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var delayTask = Task.Delay(interval, cancellationToken);
                var cancelledTable = _restaraunt.CancelBookingTimed();
                if (cancelledTable is not null)
                {
                    _producer.Send(SpeakerBotLines.AutoCancellation + cancelledTable.Id);
                }
                await delayTask;
            }
        }
    }
}
