using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bot.Data;
using Bot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Services
{
    public class NotifierService : BackgroundService
    {
        private readonly ITelegramBotClient _bot;
        private readonly AppDbContext _context; 

        public NotifierService(ITelegramBotClient bot, AppDbContext context)
        {
            _bot = bot;
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var previousDate = now.AddDays(-7);

                var lastWeekOrders = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.OrderDate > previousDate)
                    .Select(o => o)
                    .ToListAsync(stoppingToken);

                var owesPerUsersId = new Dictionary<int, decimal>();

                foreach (var order in lastWeekOrders)
                {
                    var orderTakeouts = await _context.OrderParts
                        .AsNoTracking()
                        .Where(op => op.OrderId == order.Id)
                        .Where(op => op.TakenDate > previousDate)
                        .GroupBy(op => op.UserId)
                        .ToDictionaryAsync(group => group.Key, group => group.Sum(op => op.Amount), stoppingToken);

                    var pricePerGram = order.Price / order.Amount;

                    foreach (var (userId, amountTakenFromOrder) in orderTakeouts)
                    {
                        var pricePerOrder = amountTakenFromOrder * pricePerGram;

                        if (owesPerUsersId.ContainsKey(userId))
                        {
                            owesPerUsersId[userId] += pricePerOrder;
                        }
                        else
                        {
                            owesPerUsersId.Add(userId, pricePerOrder);
                        }
                    }
                }

                foreach (var (userId, weekOwe) in owesPerUsersId)
                {
                    await _bot.SendTextMessageAsync(
                        new ChatId(userId),
                        )
                }
                
                var daysUntilMonday = (DayOfWeek.Monday - now.DayOfWeek + 7) % 7;
                
                
            }
            
            
        }
        
        private record OrderInfo(Order Order, decimal PricePerGram);
    }
}
