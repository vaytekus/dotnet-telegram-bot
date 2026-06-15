namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IRateQueryService
    {
        Task<bool> TryHandleAsync(long chatId, string currency, string dateInput, CancellationToken ct);
    }
}