using System;
using System.Linq;
using System.Threading.Tasks;
using Bot.Data;
using Bot.Helpers;
using Bot.Models;
using Bot.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = Bot.Models.User;

namespace Bot.Services
{
    public interface IMessageService
    {
        Task HandleAsync(Message message);
    }
    
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;
        private readonly ITelegramBotClient _bot;
        private readonly TelegramSettings _telegramSettings;
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        public MessageService(AppDbContext context, ITelegramBotClient bot, IOptions<TelegramSettings> telegramSettings, OrderService orderService, UserService userService)
        {
            _context = context;
            _bot = bot;
            _orderService = orderService;
            _userService = userService;
            _telegramSettings = telegramSettings.Value;
        }

        public async Task HandleAsync(Message message)
        {
            var messageUser = message.From;
            
            await _userService.CreateIfNotExistsAsync(messageUser);

            if (message.Text.StartsWith("/start", StringComparison.InvariantCultureIgnoreCase))
            {
                var orders = await _orderService.ListAsync();

                await _bot.SendTextMessageAsync(new(message.Chat.Id),
                    "Select order",
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
                            "Wrong data provided. Message template is: /neworder amount price");
                        
                        return;
                    }

                    await _orderService.CreateAsync(amount, price);
                }
                else
                {
                    await _bot.SendTextMessageAsync(
                        new(messageUser.Id),
                        "You are not allowed to create new orders!");
                }
            }
        }
    }
}
