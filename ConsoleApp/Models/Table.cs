using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Models
{
    /// <summary>
    /// Класс для сущности "Стол в ресторане"
    /// </summary>
    internal class Table
    {
        public State State { get; private set; }
        public int SeatsCount { get; }
        public int Id { get; }

        public Table (int id)
        {
            Id = id;
            State = State.Free;
            Random rand = new Random();
            SeatsCount = rand.Next(2, 5);
        }
        /// <summary>
        /// Назначение состояния
        /// </summary>
        /// <param name="state">Состояние, которое нужно назначить</param>
        /// <returns></returns>
        public bool SetState(State state)
        {
            if (state == State)
            {
                return false;
            }

            State = state;
            return true;
        }
    }
}
