using System.Linq;
using System.Threading.Tasks;
using Bot.Helpers;
using Bot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Services
{
    public interface ICallbackQueryService
    {
        Task HandleAsync(CallbackQuery callbackQuery);
    }

    public class CallbackQueryService : ICallbackQueryService
    {
        private readonly ITelegramBotClient _bot;
        private readonly OrderPartService _orderPartService;
        private readonly OrderService _orderService;

        public CallbackQueryService(ITelegramBotClient bot, OrderPartService orderPartService, OrderService orderService)
        {
            _bot = bot;
            _orderPartService = orderPartService;
            _orderService = orderService;
        }

        public async Task HandleAsync(CallbackQuery callbackQuery)
        {
            var callbackQueryUser = callbackQuery.From;

            var callbackQueryData = callbackQuery.Data.Split(".").Select(int.Parse).ToList();

            var order = await _orderService.GetAsync(callbackQueryData[0], callbackQueryUser.Id);

            var amount = decimal.Zero;

            if (callbackQueryData.Count == 2)
            {
                amount = callbackQueryData[1] switch
                {
                    Constants.UnitCallbackQueryData => 1M,
                    Constants.HalfCallbackQueryData => 0.5M,
                    Constants.QuarterCallbackQueryData => 0.25M,
                    Constants.PointFifteenCallbackQueryData => 0.15M,
                    _ => decimal.Zero
                };

                if (amount == decimal.Zero)
                {
                    var latestForUser = order.OrderParts.FirstOrDefault();

                    if (latestForUser == null)
                    {
                        await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, "You don't have any expenses to remove");
                    }
                    else
                    {
                        await _orderPartService.RemoveAsync(latestForUser);

                        await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, "Removed");
                    }
                }
                else
                {
                    await _orderPartService.CreateAsync(callbackQueryUser.Id, amount, order.Id);

                    await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, "Recorded");
                }
            }

            var totalTaken = order.OrderParts.Sum(op => op.Amount) + amount;
            var totalOwe = order.Price / order.Amount * totalTaken;

            await _bot.EditMessageTextAsync(new(callbackQueryUser.Id),
                callbackQuery.Message.MessageId,
                $"Total amount taken from current order: <b>{totalTaken}</b>\nYou owe: <b>{totalOwe}</b>",
                ParseMode.Html,
                replyMarkup: ReplyMarkupHelpers.GetAmountsMarkup(order.Id));
        }
    }
}
