using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages.Interfaces
{
    public interface IKitchenReady
    {
        public Guid OrderId { get; }
        public bool isReady { get; }
    }
}
