using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers
{
    public class WaitingForDateHandler(
        IDateValidator dateValidator,
        ITelegramBotClient bot,
        IRateQueryService rateQueryService,
        IServiceScopeFactory scopeFactory,
        ILogger<WaitingForDateHandler> logger) : IStateHandler
    {
        public UserState State => UserState.WaitingForDate;

        public async Task HandleAsync(long chatId, string text, CancellationToken ct)
        {
            if (!dateValidator.TryValidate(text.Trim(), out _, out var errorMessage))
            {
                logger.LogWarning("Date validation failed for {ChatId}: {Error}", chatId, errorMessage);
                await bot.SendMessage(chatId, errorMessage,
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                return;
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var sessionReader = scope.ServiceProvider.GetRequiredService<ISessionReader>();
            var sessionWriter = scope.ServiceProvider.GetRequiredService<ISessionWriter>();
            var session = await sessionReader.GetAsync(chatId);

            if (session?.SelectedCurrency is null)
            {
                logger.LogWarning("Session or currency missing for {ChatId}", chatId);
                await bot.SendMessage(chatId, "Сесія застаріла. Почніть з початку.",
                    cancellationToken: ct);
                return;
            }

            logger.LogInformation("User {ChatId} requested {Currency} rate for date {Date}",
                chatId, session.SelectedCurrency, text.Trim());

            await rateQueryService.TryHandleAsync(chatId, session.SelectedCurrency, text.Trim(), ct);
            await sessionWriter.SaveStateAsync(chatId, UserState.WaitingForCurrency);

            await bot.SendMessage(chatId,
                "Введіть код валюти або валюту з датою через пробіл:\n`USD` або `USD 01.06.2024`",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}