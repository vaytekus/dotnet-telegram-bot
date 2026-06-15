using Telegram.Bot.Types;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IBotErrorHandler
    {
        Task HandleAsync(Update update, Exception ex, CancellationToken ct);
    }
}