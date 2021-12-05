namespace Bot.Models;

public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal Amount { get; set; }

    public decimal Price { get; set; }

    public virtual IEnumerable<Takeout> Takeouts { get; set; }
}