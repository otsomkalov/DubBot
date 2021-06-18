using System;

namespace Bot.Models
{
    public class Takeout : BaseEntity
    {
        public DateTime Date { get; set; } = DateTime.Now;

        public decimal Amount { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }
    }
}
