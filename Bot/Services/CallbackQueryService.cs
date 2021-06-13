using System.Linq;
using System.Threading.Tasks;
using Bot.Helpers;
using Bot.Models;
using Bot.Resources;
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

        public CallbackQueryService(ITelegramBotClient bot, OrderPartService orderPartService, OrderService orderService,
            IStringLocalizer<Messages> localizer)
        {
            _bot = bot;
            _orderPartService = orderPartService;
            _orderService = orderService;
            _localizer = localizer;
        }

        public async Task HandleAsync(CallbackQuery callbackQuery)
        {
            var callbackQueryUser = callbackQuery.From;

            var callbackQueryData = callbackQuery.Data.Split("|").Select(int.Parse).ToList();

            var order = await _orderService.GetAsync(callbackQueryData[0], callbackQueryUser.Id);

            if (callbackQueryData.Count == 2)
            {
                var amountToAdd = callbackQueryData[1] switch
                {
                    Constants.UnitCallbackQueryData => 1M,
                    Constants.HalfCallbackQueryData => 0.5M,
                    Constants.QuarterCallbackQueryData => 0.25M,
                    Constants.PointFifteenCallbackQueryData => 0.15M,
                    _ => decimal.Zero
                };

                if (amountToAdd == decimal.Zero)
                {
                    var latestForUser = order.OrderParts.FirstOrDefault();

                    if (latestForUser == null)
                    {
                        await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.NoExpenses]);
                    }
                    else
                    {
                        await _orderPartService.RemoveAsync(latestForUser);

                        await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.Removed]);
                    }
                }
                else
                {
                    await _orderPartService.CreateAsync(callbackQueryUser.Id, amountToAdd, order.Id);

                    await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.Recorded]);
                }
            }

            order = await _orderService.GetAsync(callbackQueryData[0], callbackQueryUser.Id);

            var totalTaken = order.OrderParts.Sum(op => op.Amount);
            var totalOwe = order.Price / order.Amount * totalTaken;

            var takeouts = string.Join('\n', order.OrderParts.Select(op => $"{op.TakenDate:dd-MM HH:mm} - *{op.Amount}*"));

            if (string.IsNullOrEmpty(takeouts))
            {
                takeouts = "*None*";
            }

            await _bot.EditMessageTextAsync(new(callbackQueryUser.Id),
                callbackQuery.Message.MessageId, 
                string.Format(
                    _localizer[ResourcesNames.DefaultMessage],
                    order.OrderDate.ToString("dd-MM"),
                    order.Amount,
                    totalTaken,
                    totalOwe, 
                    takeouts),
                ParseMode.Markdown,
                replyMarkup: ReplyMarkupHelpers.GetAmountsMarkup(order.Id));
        }
    }
}
