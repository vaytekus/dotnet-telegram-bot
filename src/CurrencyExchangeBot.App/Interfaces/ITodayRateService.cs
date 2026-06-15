namespace CurrencyExchangeBot.App.Interfaces
{
    public interface ITodayRateService
    {
        Task<bool> HandleAsync(long chatId, string currency, CancellationToken ct);
    }
}