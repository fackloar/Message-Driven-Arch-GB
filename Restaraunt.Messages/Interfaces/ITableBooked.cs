using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages.Interfaces
{
    public interface ITableBooked
    {
        public Guid OrderId { get; }
        public bool Success { get; }
        public DateTime CreationDate { get; }
    }
}
