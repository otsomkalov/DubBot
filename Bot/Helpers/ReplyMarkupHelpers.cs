using Bot.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Helpers
{
    public static class ReplyMarkupHelpers
    {
        public static InlineKeyboardMarkup GetAmountsMarkup()
        {
            return new(
                new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton
                        {
                            Text = "+0.15",
                            CallbackData = Constants.PointFifteenCallbackQueryData
                        },
                        new InlineKeyboardButton
                        {
                            Text = "+0.25",
                            CallbackData = Constants.QuarterCallbackQueryData
                        },
                        new InlineKeyboardButton
                        {
                            Text = "+0.5",
                            CallbackData = Constants.HalfCallbackQueryData
                        },
                        new InlineKeyboardButton
                        {
                            Text = "+1",
                            CallbackData = Constants.UnitCallbackQueryData
                        }
                    },
                    new[]
                    {
                        new InlineKeyboardButton
                        {
                            Text = "Remove latest",
                            CallbackData = Constants.RemoveLatestCallbackQueryData
                        }
                    }
                });
        }
    }
}
