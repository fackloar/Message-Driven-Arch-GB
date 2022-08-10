using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Models
{
    /// <summary>
    /// класс для взаимодействия с пользователем
    /// </summary>
    internal class SpeakerBot
    {
        private readonly Restaraunt _restaraunt;
        private readonly TimeSpan interval = TimeSpan.FromMinutes(1);
        public SpeakerBot(Restaraunt restaraunt)
        {
            _restaraunt = restaraunt;

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
            initialInput:
                if (!int.TryParse(Console.ReadLine(), out var choice) && choice is not (1 or 2))
                {
                    Console.WriteLine(SpeakerBotLines.Input1Or2);
                    goto initialInput;
                }

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                if (choice == 1)
                {
                    ChooseTypeOfBooking();
                }
                else
                {
                    ChooseTypeOfCancellation();
                }
                stopWatch.Stop();
                var ts = stopWatch.Elapsed;
                Console.WriteLine($"{ts.Seconds:00}:{ts.Milliseconds:00}");
            }
        }

        private async void ChooseTypeOfBooking()
        {
            Console.WriteLine(SpeakerBotLines.BookingChoice);
        input:
            if (!int.TryParse(Console.ReadLine(), out var choiceOfBooking) && choiceOfBooking is not (1 or 2))
            {
                Console.WriteLine(SpeakerBotLines.Input1Or2);
                goto input;
            }
            Console.WriteLine(SpeakerBotLines.WaitForBooking);
            if (choiceOfBooking == 1)
            {
                Console.WriteLine(SpeakerBotLines.YoullBeNotified);
                var table = await _restaraunt.BookFreeTableAsync(1);
                NotifyOnBooking(table);
            }
            else
            {
                Console.WriteLine(SpeakerBotLines.StayOnLine);
                var table = _restaraunt.BookFreeTable(1);
                Console.WriteLine(table is null
                    ? SpeakerBotLines.AllTablesOccupied
                    : SpeakerBotLines.BookingReady + table.Id);
            }
        }

        private async void ChooseTypeOfCancellation()
        {
            Console.WriteLine(SpeakerBotLines.CancellationChoice);
        input:
            if (!int.TryParse(Console.ReadLine(), out var choiceOfCancellation) && choiceOfCancellation is not (1 or 2))
            {
                Console.WriteLine(SpeakerBotLines.Input1Or2);
                goto input;
            }
            Console.WriteLine(SpeakerBotLines.WaitForCancellation);
            if (choiceOfCancellation == 1)
            {
            inputTableId1:
                if (!int.TryParse(Console.ReadLine(), out var tableId) || (tableId < 0 || tableId > 10))
                {
                    Console.WriteLine(SpeakerBotLines.InputTableId);
                    goto inputTableId1;
                }
                if (!_restaraunt.CheckIfBooked(tableId))
                {
                    Console.WriteLine(tableId + SpeakerBotLines.TableNotOccupied);
                }
                else
                {
                    Console.WriteLine(SpeakerBotLines.YoullBeNotified);
                    var table = await _restaraunt.CancelBookingAsync(tableId);
                    NotifyOnCancellation(table);
                }
            }
            else
            {
            inputTableId2:
                if (!int.TryParse(Console.ReadLine(), out var tableId) || (tableId < 0 || tableId > 10))
                {
                    Console.WriteLine(SpeakerBotLines.InputTableId);
                    goto inputTableId2;
                }
                if (!_restaraunt.CheckIfBooked(tableId))
                {
                    Console.WriteLine(tableId + SpeakerBotLines.TableNotOccupied);
                }
                else
                {
                    Console.WriteLine(SpeakerBotLines.StayOnLine);
                    var table = _restaraunt.CancelBooking(tableId);
                    Console.WriteLine(SpeakerBotLines.CancellationReady + table.Id);
                }
            }
        }

        private async void NotifyOnBooking(Table table)
        {
            await Task.Run(async () =>
            {
                await Task.Delay(1000);
                Console.WriteLine(table is null
                    ? SpeakerBotLines.Notification + SpeakerBotLines.AllTablesOccupied
                    : SpeakerBotLines.Notification + SpeakerBotLines.BookingReady + table.Id);
            });
        }
        private async void NotifyOnCancellation(Table table)
        {
            await Task.Run(async () =>
            {
                await Task.Delay(1000);
                Console.WriteLine(SpeakerBotLines.Notification + SpeakerBotLines.CancellationReady + table.Id);
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
                    Console.WriteLine(SpeakerBotLines.AutoCancellation + cancelledTable.Id);
                }
                await delayTask;
            }
        }
    }
}
