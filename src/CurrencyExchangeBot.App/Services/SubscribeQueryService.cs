using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Services
{
    public class SubscribeQueryService(
        ITelegramBotClient bot,
        IServiceScopeFactory scopeFactory,
        ILogger<SubscribeQueryService> logger) : ISubscribeQueryService
    {
        public async Task<bool> TryHandleAsync(long chatId, string currency, string timeInput, CancellationToken ct)
        {
            if (!TimeOnly.TryParse(timeInput, out var time))
            {
                return false;
            }
            
            await using var scope = scopeFactory.CreateAsyncScope();
            var subscriptionWriter = scope.ServiceProvider.GetRequiredService<ISubscriptionWriter>();
            await subscriptionWriter.SubscribeAsync(chatId, currency, time);

            logger.LogInformation("User {ChatId} quick subscribe {Currency} at {Time}", chatId, currency, time);

            await bot.SendMessage(chatId,
                $"Підписку оформлено! Щодня о `{time:HH:mm}` надсилатиму курс *{currency}/UAH*.",
                parseMode: ParseMode.Markdown, cancellationToken: ct);

            return true;
        }
    }
}