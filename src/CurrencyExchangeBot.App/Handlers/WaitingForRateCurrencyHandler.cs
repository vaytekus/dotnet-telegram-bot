using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers
{
    public class WaitingForRateCurrencyHandler(
        ITelegramBotClient bot,
        ICurrencyValidator currencyValidator,
        ITodayRateService todayRateService,
        IServiceScopeFactory scopeFactory,
        ILogger<WaitingForRateCurrencyHandler> logger) : IStateHandler
    {
        public UserState State => UserState.WaitingForRateCurrency;

        public async Task HandleAsync(long chatId, string text, CancellationToken ct)
        {
            if (!currencyValidator.TryValidate(text, out var currency, out var errorMessage))
            {
                logger.LogWarning("Invalid rate currency input from {ChatId}: {Input}", chatId, text);
                await bot.SendMessage(chatId, errorMessage,
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                return;
            }
            
            var success = await todayRateService.HandleAsync(chatId, currency, ct);

            if (success)
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var sessionWriter = scope.ServiceProvider.GetRequiredService<ISessionWriter>();
                await sessionWriter.ResetAsync(chatId);
            }
        }
    }
}