using Bot.Data;
using Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.Services;

public class TakeoutService
{
    private readonly AppDbContext _context;

    public TakeoutService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RemoveAsync(Takeout takeout)
    {
        _context.Remove(takeout);
        await _context.SaveChangesAsync();
    }

    public async Task CreateAsync(long userId, decimal amount, long orderId)
    {
        await _context.AddAsync(new Takeout
        {
            UserId = userId,
            Amount = amount,
            OrderId = orderId
        });

        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<Takeout>> ListSinceAsync(long userId, DateTime since)
    {
        return await _context.Takeouts
            .AsNoTracking()
            .Include(t => t.Order)
            .Where(t => t.UserId == userId)
            .Where(t => t.Date >= since)
            .OrderBy(t => t.Date)
            .ToListAsync();
    }
}