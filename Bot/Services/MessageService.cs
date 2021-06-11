using System;
using System.Threading.Tasks;
using Bot.Helpers;
using Bot.Resources;
using Bot.Settings;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Services
{
    public class MessageService
    {
        private readonly ITelegramBotClient _bot;
        private readonly TelegramSettings _telegramSettings;
        private readonly OrderService _orderService;
        private readonly UserService _userService;
        private readonly IStringLocalizer<Messages> _localizer;

        public MessageService(ITelegramBotClient bot, IOptions<TelegramSettings> telegramSettings, OrderService orderService,
            UserService userService, IStringLocalizer<Messages> localizer)
        {
            _bot = bot;
            _orderService = orderService;
            _userService = userService;
            _localizer = localizer;
            _telegramSettings = telegramSettings.Value;
        }

        public async Task HandleAsync(Message message)
        {
            var messageUser = message.From;
            
            await _userService.CreateIfNotExistsAsync(messageUser);

            if (message.Text.StartsWith("/start", StringComparison.InvariantCultureIgnoreCase))
            {
                var orders = await _orderService.ListAsync();

                await _bot.SendTextMessageAsync(
                    new(message.Chat.Id),
                    _localizer[ResourcesNames.SelectOrder],
                    replyMarkup: ReplyMarkupHelpers.GetOrdersMarkup(orders));
                
                return;
            }

            if (message.Text.StartsWith("/neworder", StringComparison.InvariantCultureIgnoreCase))
            {
                if (messageUser.Id.ToString() == _telegramSettings.AdminId)
                {
                    var messageParts = message.Text.Split(" ");

                    if (messageParts.Length != 3 || !decimal.TryParse(messageParts[1], out var amount) ||
                        !decimal.TryParse(messageParts[2], out var price))
                    {
                        await _bot.SendTextMessageAsync(
                            new(messageUser.Id),
                            _localizer[ResourcesNames.WrongNewOrder]);
                        
                        return;
                    }

                    await _orderService.CreateAsync(amount, price);
                }
                else
                {
                    await _bot.SendTextMessageAsync(
                        new(messageUser.Id),
                        _localizer[ResourcesNames.NotAuthorized]);
                }
            }
        }
    }
}
