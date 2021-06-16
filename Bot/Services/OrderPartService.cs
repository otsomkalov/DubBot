using System.Linq;
using System.Threading.Tasks;
using Bot.Data;
using Bot.Models;
using Microsoft.EntityFrameworkCore;
using User = Telegram.Bot.Types.User;

namespace Bot.Services
{
    public class OrderPartService
    {
        private readonly AppDbContext _context;

        public OrderPartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderPart> GetLatestForUserAsync(User user)
        {
            return await _context.OrderParts
                .AsNoTracking()
                .Where(op => op.UserId == user.Id)
                .OrderByDescending(op => op.TakenDate)
                .FirstOrDefaultAsync();            
        }

        public async Task RemoveAsync(OrderPart orderPart)
        {
            _context.Remove(orderPart);
            await _context.SaveChangesAsync();
        }

        public async Task<OrderPart> CreateAsync(int id, decimal amount, int orderId)
        {
            var createdEntity = await _context.AddAsync(new OrderPart
            {
                UserId = id,
                Amount = amount,
                OrderId = orderId
            });

            await _context.SaveChangesAsync();

            return createdEntity.Entity;
        }
    }
}
