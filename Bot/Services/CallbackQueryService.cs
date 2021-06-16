using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Data;
using Bot.Helpers;
using Bot.Models;
using Bot.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Services
{
    public class CallbackQueryService
    {
        private readonly ITelegramBotClient _bot;
        private readonly OrderPartService _orderPartService;
        private readonly OrderService _orderService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly AppDbContext _context;

        public CallbackQueryService(ITelegramBotClient bot, OrderPartService orderPartService, OrderService orderService,
            IStringLocalizer<Messages> localizer, AppDbContext context)
        {
            _bot = bot;
            _orderPartService = orderPartService;
            _orderService = orderService;
            _localizer = localizer;
            _context = context;
        }

        public async Task HandleAsync(CallbackQuery callbackQuery)
        {
            var callbackQueryUser = callbackQuery.From;

            var now = DateTime.UtcNow.Date;

            var daysSinceSunday = now.DayOfWeek - DayOfWeek.Sunday;

            var sundayDate = now.AddDays(daysSinceSunday);

            var orderPartsSinceSunday = await _context.OrderParts
                .AsNoTracking()
                .Include(op => op.Order)
                .Where(op => op.TakenDate > sundayDate)
                .Where(op => op.UserId == callbackQueryUser.Id)
                .OrderBy(op => op.TakenDate)
                .GroupBy(op => op.Order)
                .ToDictionaryAsync(gr => gr.Key, gr => gr);

            var messageBuilder = new StringBuilder();

            foreach (var (order, orderParts) in orderPartsSinceSunday)
            {
                var pricePerGram = order.Price / order.Amount;

                foreach (var orderPart in orderParts)
                {
                    messageBuilder.Append($"{orderPart.TakenDate:dd-MM HH:mm} - {orderPart.Amount} g.")
                }
            }

            var latestOrder = await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();
            
            var amountToAdd = callbackQuery.Data switch
            {
                Constants.PointFifteenCallbackQueryData => 0.15M,
                Constants.QuarterCallbackQueryData => 0.25M,
                Constants.HalfCallbackQueryData => 0.5M,
                Constants.UnitCallbackQueryData => 1M,
                _ => decimal.Zero
            };
            
            if (amountToAdd == decimal.Zero)
            {
                var latestForUser = orderPartsSinceSunday.LastOrDefault();

                if (latestForUser == null)
                {
                    await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.NoExpenses]);
                }
                else
                {
                    await _orderPartService.RemoveAsync(latestForUser);

                    await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.Removed]);

                    orderPartsSinceSunday.Remove(latestForUser);
                }
            }
            else
            {
                var createdOrderPart = await _orderPartService.CreateAsync(callbackQueryUser.Id, amountToAdd, latestOrder.Id);

                await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.Recorded]);
                
                orderPartsSinceSunday.Add(createdOrderPart);
            }

            var takeouts = string.Join('\n', orderPartsSinceSunday.Select(op => $"{op.TakenDate:dd-MM HH:mm} - *{op.Amount}*"));

            if (string.IsNullOrEmpty(takeouts))
            {
                takeouts = "*None*";
            }

            await _bot.EditMessageTextAsync(new(callbackQueryUser.Id),
                callbackQuery.Message.MessageId, 
                string.Format(
                    _localizer[ResourcesNames.DefaultMessage],
                    sundayDate.ToString("dd-MM"),
                    takeouts),
                ParseMode.Markdown,
                replyMarkup: ReplyMarkupHelpers.GetAmountsMarkup());
        }
    }
}
