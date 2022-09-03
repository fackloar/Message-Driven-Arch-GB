﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages.Interfaces
{
    public class IBookingRequest
    {
        public Guid OrderId { get; }

        public Guid ClientId { get; }

        public Dish? PreOrder { get; }

        public DateTime CreationDate { get; }
    }
}
