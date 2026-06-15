using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers.Commands
{
    public class StartCommandHandler(
        ITelegramBotClient bot, 
        IServiceScopeFactory scopeFactory) : ICommandHandler
    {
        public string Command => "/start";
        public async Task HandleAsync(long chatId, string message, CancellationToken ct)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var sessionWriter = scope.ServiceProvider.GetRequiredService<ISessionWriter>();

            await sessionWriter.ResetAsync(chatId);
            await sessionWriter.SaveStateAsync(chatId, UserState.WaitingForCurrency);

            await bot.SendMessage(chatId,
                "Введіть код валюти або валюту з датою через пробіл:\n`USD` або `USD 01.06.2024`",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}