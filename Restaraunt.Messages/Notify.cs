using Restaraunt.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages
{
    internal class Notify : INotify
    {
        public Notify(Guid orderId, Guid clientId, string message)
        {
            OrderId = orderId;
            ClientId = clientId;
            Message = message;
        }

        public Guid OrderId { get; }
        public Guid ClientId { get; }
        public string Message { get; }
    }
}
