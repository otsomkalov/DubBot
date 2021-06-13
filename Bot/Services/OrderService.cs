using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Data;
using Bot.Models;
using Microsoft.EntityFrameworkCore;
using User = Telegram.Bot.Types.User;

namespace Bot.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetLatestForUserAsync(User user)
        {
            return await _context.Orders
                .Include(o => o.OrderParts.Where(op => op.UserId == user.Id).OrderByDescending(op => op.TakenDate))
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();       
        }

        public async Task CreateAsync(decimal amount, decimal price)
        {
            await _context.AddAsync(
                new Order
                {
                    Amount = amount,
                    Price = price
                });

            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<Order>> ListAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public Task<Order> GetAsync(int id, int userId)
        {
            return _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderParts.Where(op => op.UserId == userId).OrderByDescending(op => op.TakenDate))
                .SingleOrDefaultAsync(o => o.Id == id);
        }
    }
}
