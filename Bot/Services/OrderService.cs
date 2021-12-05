using Bot.Data;
using Microsoft.EntityFrameworkCore;

namespace Bot.Services;

public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(decimal amount, decimal price)
    {
        await _context.AddAsync(new Order
        {
            Amount = amount,
            Price = price
        });

        await _context.SaveChangesAsync();
    }

    public async Task<Order> GetLatestAsync()
    {
        return await _context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .FirstOrDefaultAsync();
    }
}