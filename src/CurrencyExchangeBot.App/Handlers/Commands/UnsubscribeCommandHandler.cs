using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace CurrencyExchangeBot.App.Handlers.Commands
{
    public class UnsubscribeCommandHandler(ITelegramBotClient bot, IServiceScopeFactory scopeFactory) : ICommandHandler
    {
        public string Command => "/unsubscribe";

        public async Task HandleAsync(long chatId, string message, CancellationToken ct)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var subscriptionReader = scope.ServiceProvider.GetRequiredService<ISubscriptionReader>();
            var subscriptionWriter = scope.ServiceProvider.GetRequiredService<ISubscriptionWriter>();
            var subscription = await subscriptionReader.GetSubscriptionAsync(chatId);

            if (subscription is null)
            {
                await bot.SendMessage(chatId,
                    "У вас немає активної підписки.",
                    cancellationToken: ct);
                return;
            }

            await subscriptionWriter.UnsubscribeAsync(chatId);
            await bot.SendMessage(chatId,
                "Підписку скасовано. Ви більше не отримуватимете сповіщення.",
                cancellationToken: ct); 
        }
    }
}