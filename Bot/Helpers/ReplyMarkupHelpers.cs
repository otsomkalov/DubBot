using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Helpers;

public static class ReplyMarkupHelpers
{
    public static InlineKeyboardMarkup GetAmountsMarkup()
    {
        return new(
            new[]
            {
                new[]
                {
                    new InlineKeyboardButton("+0.15")
                    {
                        CallbackData = Constants.PointFifteenCallbackQueryData
                    },
                    new InlineKeyboardButton("+0.25")
                    {
                        CallbackData = Constants.QuarterCallbackQueryData
                    },
                    new InlineKeyboardButton("+0.5")
                    {
                        CallbackData = Constants.HalfCallbackQueryData
                    },
                    new InlineKeyboardButton("+1")
                    {
                        CallbackData = Constants.UnitCallbackQueryData
                    }
                },
                new[]
                {
                    new InlineKeyboardButton("Remove latest")
                    {
                        CallbackData = Constants.RemoveLatestCallbackQueryData
                    }
                }
            });
    }
}