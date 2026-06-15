using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CurrencyExchangeBot.App.Handlers.Exceptions
{
    public class SqlExceptionHandler(
        ITelegramBotClient bot,
        ILogger<SqlExceptionHandler> logger) : IExceptionHandler
    {
        public bool CanHandle(Exception ex) => ex is SqlException;

        public async Task HandleAsync(long? chatId, Exception ex, CancellationToken ct)
        {
            logger.LogError(ex, "Database unavailable");
            if (chatId is not null)
            {
                await bot.SendMessage(chatId.Value,
                    "Сервіс тимчасово недоступний. Спробуйте пізніше.",
                    cancellationToken: ct);
            }
        }
    }
}
