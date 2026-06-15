using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers
{
    public class WaitingForSubscribeCurrencyHandler(
        ITelegramBotClient bot,
        ICurrencyValidator currencyValidator,
        IServiceScopeFactory scopeFactory,
        ISubscribeQueryService subscribeQueryService,
        ILogger<WaitingForSubscribeCurrencyHandler> logger) : IStateHandler
    {
        public UserState State => UserState.WaitingForSubscribeCurrency;

        public async Task HandleAsync(long chatId, string text, CancellationToken ct)
        {
            var parts = text.Trim().Split(" ", 2);
            if (!currencyValidator.TryValidate(parts[0], out var currency, out var errorMessage))
            {
                logger.LogWarning("Invalid subscribe currency input from {ChatId}: {Input}", chatId, text);
                await bot.SendMessage(chatId, errorMessage,
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                return;
            }
            
            if (parts.Length == 2 && await subscribeQueryService.TryHandleAsync(chatId, currency, parts[1].Trim(), ct))
            {
                return;
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var sessionWriter = scope.ServiceProvider.GetRequiredService<ISessionWriter>();

            await sessionWriter.SaveCurrencyAsync(chatId, currency);
            await sessionWriter.SaveStateAsync(chatId, UserState.WaitingForSubscribeTime);

            logger.LogInformation("User {ChatId} entered subscribe currency {Currency}", chatId, currency);

            await bot.SendMessage(chatId,
                $"Ви вибрали *{currency}*. Введіть час у форматі `HH:mm`:",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}
