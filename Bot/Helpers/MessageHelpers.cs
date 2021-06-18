using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bot.Models;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Helpers
{
    public static class MessageHelpers
    {
        public static string GetTakeoutsMessage(this IStringLocalizer<Messages> localizer, ICollection<Takeout> takeouts)
        {
            var messageBuilder = new StringBuilder();
            var totalTaken = .0M;
            var totalOwe = .0M;

            if (takeouts.Any())
            {
                foreach (var takeout in takeouts)
                {
                    var pricePerGram = takeout.Order.Price / takeout.Order.Amount;
                    var takeoutPrice = takeout.Amount * pricePerGram;

                    totalOwe += takeoutPrice;
                    totalTaken += takeout.Amount;

                    messageBuilder.AppendLine(string.Format(localizer[ResourcesNames.TakeoutInfo],
                        takeout.Date.ToString("dd-MM HH:mm"),
                        takeout.Amount,
                        takeoutPrice));
                }
            }
            else
            {
                messageBuilder.AppendLine("*None*");
            }

            return string.Format(localizer[ResourcesNames.DefaultMessage],
                messageBuilder,
                totalTaken,
                totalOwe);
        }
    }
}
