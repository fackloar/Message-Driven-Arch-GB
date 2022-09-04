using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Booking.Classes
{
    /// <summary>
    /// Класс для сущности "Ресторан"
    /// </summary>
    public class RestarauntClass
    {
        private readonly List<Table> _tables = new();
        private readonly AutoResetEvent _autoResetEvent = new(true);

        public RestarauntClass()
        {
            for (ushort i = 1; i <= 10; i++)
            {
                _tables.Add(new Table(i));
            }


        }
        /// <summary>
        /// Забронировать свободный столик по количеству гостей, асинхронно
        /// </summary>
        /// <param name="countOfGuests">количество гостей</param>
        /// <returns>забронированный стол</returns>
        public async Task<bool?> BookFreeTableAsync(int countOfGuests)
        {
            _autoResetEvent.WaitOne();
            var table = _tables.FirstOrDefault(t => t.SeatsCount > countOfGuests && t.State == State.Free);
            _autoResetEvent.Set();
            return table?.SetState(State.Booked);
        }
        /// <summary>
        /// отменить бронь стола, синхронно
        /// </summary>
        /// <param name="tableId">номер стола</param>
        /// <returns>стол, на котором была отменена бронь</returns>
        public async Task<bool?> CancelBookingAsync(int tableId)
        {
            var table = _tables.Find(t => t.Id == tableId);
            await Task.Delay(1000 * 5);
            return table?.SetState(State.Free);

        }
        /// <summary>
        /// проверка, забронирован ли стол
        /// </summary>
        /// <param name="tableId">номер стола</param>
        /// <returns></returns>
        public bool CheckIfBooked(int tableId)
        {
            var table = _tables.Find(t => t.Id == tableId);
            return table.State == State.Booked;
        }
    }
}
