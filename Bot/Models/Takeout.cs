namespace Bot.Models;

public class Takeout : BaseEntity
{
    public DateTime Date { get; set; } = DateTime.Now;

    public decimal Amount { get; set; }

    public long UserId { get; set; }

    public virtual User User { get; set; }

    public long OrderId { get; set; }

    public virtual Order Order { get; set; }
}