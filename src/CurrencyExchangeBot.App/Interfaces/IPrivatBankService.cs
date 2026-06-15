using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IPrivatBankService
    {
        ValueTask<Result<ExchangeRate>> GetRateAsync(string currency, DateTime date, CancellationToken ct);
    }
}