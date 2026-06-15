namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IExceptionHandler
    {
        bool CanHandle(Exception ex);
        Task HandleAsync(long? chatId, Exception ex, CancellationToken ct);
    }
}