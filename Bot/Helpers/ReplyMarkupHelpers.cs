using System.Collections.Generic;
using System.Linq;
using Bot.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Helpers
{
    public static class ReplyMarkupHelpers
    {
        private const int ColumnsCount = 4;

        public static InlineKeyboardMarkup GetOrdersMarkup(ICollection<Order> orders)
        {
            var buttonsRows = new List<IEnumerable<InlineKeyboardButton>>();

            for (var i = 0; i < orders.Count; i += ColumnsCount)
            {
                var buttonsRow = orders
                    .Skip(i)
                    .Take(ColumnsCount)
                    .Select(order => new InlineKeyboardButton
                    {
                        Text = $"{order.Id.ToString()}. {order.Amount} gr.",
                        CallbackData = order.Id.ToString()
                    });

                buttonsRows.Add(buttonsRow);
            }

            return new(buttonsRows);
        }

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
