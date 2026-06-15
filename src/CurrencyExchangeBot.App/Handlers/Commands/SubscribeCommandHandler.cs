using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers.Commands
{
    public class SubscribeCommandHandler(
        ITelegramBotClient bot,
        IServiceScopeFactory scopeFactory) : ICommandHandler
    {
        public string Command => "/subscribe";

        public async Task HandleAsync(long chatId, string text, CancellationToken ct)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var sessionWriter = scope.ServiceProvider.GetRequiredService<ISessionWriter>();

            await sessionWriter.SaveStateAsync(chatId, UserState.WaitingForSubscribeCurrency);

            await bot.SendMessage(chatId,
                "Введіть код валюти або валюту з часом через пробіл:\n`USD` або `USD 08:00`",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}
