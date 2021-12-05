using Bot.Extensions;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Services;

public class CallbackQueryService
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly OrderService _orderService;
    private readonly TakeoutService _takeoutService;

    public CallbackQueryService(ITelegramBotClient bot, TakeoutService takeoutService,
        IStringLocalizer<Messages> localizer, OrderService orderService)
    {
        _bot = bot;
        _takeoutService = takeoutService;
        _localizer = localizer;
        _orderService = orderService;
    }

    public async Task HandleAsync(CallbackQuery callbackQuery)
    {
        var callbackQueryUser = callbackQuery.From;
        var sundayDate = DateTime.Now.GetLastSunday();

        var amountToAdd = callbackQuery.Data switch
        {
            Constants.PointFifteenCallbackQueryData => 0.15M,
            Constants.QuarterCallbackQueryData => 0.25M,
            Constants.HalfCallbackQueryData => 0.5M,
            Constants.UnitCallbackQueryData => 1M,
            _ => decimal.Zero
        };

        ICollection<Takeout> takeoutsSinceSunday;

        if (amountToAdd == decimal.Zero)
        {
            takeoutsSinceSunday = await _takeoutService.ListSinceAsync(callbackQueryUser.Id, sundayDate);
            var latestForUser = takeoutsSinceSunday.LastOrDefault();

            if (latestForUser == null)
            {
                await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.NoExpenses]);
            }
            else
            {
                await _takeoutService.RemoveAsync(latestForUser);

                await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.Removed]);

                takeoutsSinceSunday.Remove(latestForUser);
            }
        }
        else
        {
            var lastOrder = await _orderService.GetLatestAsync();

            await _takeoutService.CreateAsync(callbackQueryUser.Id, amountToAdd, lastOrder.Id);

            await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, _localizer[ResourcesNames.Recorded]);

            takeoutsSinceSunday = await _takeoutService.ListSinceAsync(callbackQueryUser.Id, sundayDate);
        }

        await _bot.EditMessageTextAsync(new(callbackQueryUser.Id),
            callbackQuery.Message.MessageId,
            _localizer.GetTakeoutsMessage(takeoutsSinceSunday),
            ParseMode.Markdown,
            replyMarkup: ReplyMarkupHelpers.GetAmountsMarkup());
    }
}