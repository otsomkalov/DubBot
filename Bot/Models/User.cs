namespace Bot.Models;

public class User : BaseEntity
{
    public string Username { get; set; }

    public string FirstName { get; set; }

    public int SplitwiseId { get; set; }
}