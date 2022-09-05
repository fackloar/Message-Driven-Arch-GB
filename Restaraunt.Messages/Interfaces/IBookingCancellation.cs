using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages.Interfaces
{
    public interface IBookingCancellation
    {
        public Guid OrderId { get; }
        public int? TableId { get; }
    }
}
