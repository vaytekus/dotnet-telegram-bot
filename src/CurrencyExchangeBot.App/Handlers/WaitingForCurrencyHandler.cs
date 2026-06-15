using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers
{
    public class WaitingForCurrencyHandler(
        ITelegramBotClient bot,
        ICurrencyValidator currencyValidator,
        IServiceScopeFactory scopeFactory,
        IRateQueryService rateQueryService,
        ILogger<WaitingForCurrencyHandler> logger) : IStateHandler
    {
        public UserState State => UserState.WaitingForCurrency;

        public async Task HandleAsync(long chatId, string text, CancellationToken ct)
        {
            var parts = text.Trim().Split(" ", 2);
            
            if (!currencyValidator.TryValidate(parts[0], out var currency, out var errorMessage))
            {
                logger.LogWarning("Invalid currency input from {ChatId}: {Input}", chatId, text);
                await bot.SendMessage(chatId, errorMessage,
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                return;
            }

            if (parts.Length == 2 && await rateQueryService.TryHandleAsync(chatId, currency, parts[1].Trim(), ct))
            {
                return;
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var sessionWriter = scope.ServiceProvider.GetRequiredService<ISessionWriter>();

            await sessionWriter.SaveCurrencyAsync(chatId, currency);
            await sessionWriter.SaveStateAsync(chatId, UserState.WaitingForDate);

            logger.LogInformation("User {ChatId} entered currency {Currency}", chatId, currency);

            await bot.SendMessage(chatId,
                $"Ви вибрали *{currency}*. Введіть дату у форматі `dd.MM.yyyy`:",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}