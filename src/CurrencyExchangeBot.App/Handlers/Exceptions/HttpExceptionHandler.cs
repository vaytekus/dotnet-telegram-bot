using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CurrencyExchangeBot.App.Handlers.Exceptions
{
    public class HttpExceptionHandler(
        ITelegramBotClient bot,
        ILogger<HttpExceptionHandler> logger) : IExceptionHandler
    {
        public bool CanHandle(Exception ex)
            => ex is HttpRequestException or TaskCanceledException;

        public async Task HandleAsync(long? chatId, Exception ex, CancellationToken ct)
        {
            var message = ex is TaskCanceledException 
                ? "Запит перевищив час очікування. Спробуйте пізніше." 
                : "Сервіс тимчасово недоступний. Спробуйте пізніше.";
            
            logger.LogError(ex, "HTTP error for {ChatId}", chatId);

            if (chatId is not null)
            {
                await bot.SendMessage(chatId.Value, message, cancellationToken: ct);
            }
        }
    }
}