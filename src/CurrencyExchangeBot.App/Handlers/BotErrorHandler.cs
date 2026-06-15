using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace CurrencyExchangeBot.App.Handlers
{
    public class BotErrorHandler(
        IEnumerable<IExceptionHandler> exceptionHandlers,
        ILogger<BotErrorHandler> logger) : IBotErrorHandler
    {
        public async Task HandleAsync(Update update, Exception ex, CancellationToken ct)
        {
            var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id;
            var handler = exceptionHandlers.FirstOrDefault(h => h.CanHandle(ex));

            if (handler is not null)
            {
                await handler.HandleAsync(chatId, ex, ct);
                return;
            }

            logger.LogError(ex, "Handler error");
        }
    }
}