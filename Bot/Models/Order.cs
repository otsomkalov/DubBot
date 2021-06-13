﻿using System;
using System.Collections.Generic;

namespace Bot.Models
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public decimal Amount { get; set; }

        public decimal Price { get; set; }

        public virtual IEnumerable<OrderPart> OrderParts { get; set; }
    }
}