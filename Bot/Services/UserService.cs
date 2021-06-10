using System.Threading.Tasks;
using Bot.Data;
using Bot.Models;
using Microsoft.EntityFrameworkCore;
using TG = Telegram.Bot.Types;

namespace Bot.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateIfNotExistsAsync(TG.User user)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == user.Id);
            
            if (userExists)
            {
                return;
            }

            await _context.AddAsync(
                new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName
                });

            await _context.SaveChangesAsync();
        }
    }
}
