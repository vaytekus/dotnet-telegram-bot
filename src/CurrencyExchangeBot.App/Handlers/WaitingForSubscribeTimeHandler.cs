using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers
{
    public class WaitingForSubscribeTimeHandler(
        ITelegramBotClient bot,
        IServiceScopeFactory scopeFactory,
        ILogger<WaitingForSubscribeTimeHandler> logger) : IStateHandler
    {
        public UserState State => UserState.WaitingForSubscribeTime;

        public async Task HandleAsync(long chatId, string text, CancellationToken ct)
        {
            if (!TimeOnly.TryParse(text.Trim(), out var time))
            {
                logger.LogWarning("Invalid time format from {ChatId}: {Text}", chatId, text);
                await bot.SendMessage(chatId,
                    "Невірний формат. Введіть час у форматі `HH:mm`, наприклад `08:00`",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                return;
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var sessionReader = scope.ServiceProvider.GetRequiredService<ISessionReader>();
            var sessionWriter = scope.ServiceProvider.GetRequiredService<ISessionWriter>();
            var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionWriter>();
            var session = await sessionReader.GetAsync(chatId);

            if (session?.SelectedCurrency is null)
            {
                logger.LogWarning("Session or currency missing for {ChatId}", chatId);
                await bot.SendMessage(chatId, "Сесія застаріла. Почніть з початку.",
                    cancellationToken: ct);
                return;
            }

            await subscriptionService.SubscribeAsync(chatId, session.SelectedCurrency, time);
            await sessionWriter.ResetAsync(chatId);

            logger.LogInformation("User {ChatId} subscribed to {Currency} at {Time}",
                chatId, session.SelectedCurrency, time);

            await bot.SendMessage(chatId,
                $"Підписку оформлено! Щодня о `{time:HH:mm}` надсилатиму курс *{session.SelectedCurrency}/UAH*.",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}