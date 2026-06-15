using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers.Commands
{
    public class RateCommandHandler(
        ITelegramBotClient bot,
        IServiceScopeFactory scopeFactory) : ICommandHandler
    {
        public string Command => "/rate";

        public async Task HandleAsync(long chatId, string text, CancellationToken ct)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var sessionWriter = scope.ServiceProvider.GetRequiredService<ISessionWriter>();
            await sessionWriter.SaveStateAsync(chatId, UserState.WaitingForRateCurrency);

            await bot.SendMessage(chatId,
                "Введіть код валюти (наприклад: `USD`, `EUR`, `CHF`):",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }

    }
}
