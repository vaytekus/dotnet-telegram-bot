namespace CurrencyExchangeBot.App.Interfaces
{
    public interface ISubscribeQueryService
    {
        Task<bool> TryHandleAsync(long chatId, string currency, string timeInput, CancellationToken ct);
    }
}