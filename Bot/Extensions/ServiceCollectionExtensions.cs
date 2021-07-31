using Bot.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Splitwise.Clients;
using Splitwise.Clients.Interfaces;
using Telegram.Bot;

namespace Bot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTelegram(this IServiceCollection services)
        {
            return services.AddSingleton<ITelegramBotClient>(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<TelegramSettings>>().Value;

                return new TelegramBotClient(settings.Token);
            });
        }

        public static IServiceCollection AddSplitwise(this IServiceCollection services)
        {
            return services.AddSingleton<ISplitwiseClient>(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<SplitwiseSettings>>().Value;

                return new SplitwiseClient(settings.Key);
            });
        }
    }
}