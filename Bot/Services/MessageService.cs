using System;
using System.Threading.Tasks;
using Bot.Helpers;
using Bot.Resources;
using Bot.Settings;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Services
{
    public class MessageService
    {
        private readonly ITelegramBotClient _bot;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly OrderService _orderService;
        private readonly TakeoutService _takeoutService;
        private readonly TelegramSettings _telegramSettings;
        private readonly UserService _userService;

        public MessageService(ITelegramBotClient bot, IOptions<TelegramSettings> telegramSettings, OrderService orderService,
            UserService userService, IStringLocalizer<Messages> localizer, TakeoutService takeoutService)
        {
            _bot = bot;
            _orderService = orderService;
            _userService = userService;
            _localizer = localizer;
            _takeoutService = takeoutService;
            _telegramSettings = telegramSettings.Value;
        }

        public async Task HandleAsync(Message message)
        {
            var messageUser = message.From;

            await _userService.CreateIfNotExistsAsync(messageUser);

            if (message.Text.StartsWith("/start", StringComparison.InvariantCultureIgnoreCase))
            {
                var sundayDate = DateTime.Now.GetLastSunday();
                var takeoutsSinceSunday = await _takeoutService.ListSinceAsync(messageUser.Id, sundayDate);

                await _bot.SendTextMessageAsync(new(messageUser.Id),
                    _localizer.GetTakeoutsMessage(takeoutsSinceSunday),
                    ParseMode.Markdown,
                    replyMarkup: ReplyMarkupHelpers.GetAmountsMarkup());

                return;
            }

            if (message.Text.StartsWith("/neworder", StringComparison.InvariantCultureIgnoreCase))
            {
                if (messageUser.Id.ToString() == _telegramSettings.AdminId)
                {
                    var messageParts = message.Text.Split(" ");

                    if (messageParts.Length != 3 ||
                        !decimal.TryParse(messageParts[1], out var amount) ||
                        !decimal.TryParse(messageParts[2], out var price))
                    {
                        await _bot.SendTextMessageAsync(
                            new(messageUser.Id),
                            _localizer[ResourcesNames.WrongNewOrder]);

                        return;
                    }

                    await _orderService.CreateAsync(amount, price);

                    await _bot.SendTextMessageAsync(new(messageUser.Id),
                        _localizer[ResourcesNames.OrderCreated]);
                }
                else
                {
                    await _bot.SendTextMessageAsync(new(messageUser.Id),
                        _localizer[ResourcesNames.NotAuthorized]);
                }
            }
        }
    }
}
