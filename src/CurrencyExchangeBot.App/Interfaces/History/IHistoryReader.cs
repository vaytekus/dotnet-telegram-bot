using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IHistoryReader
    {
        Task<List<ExchangeRateHistory>> GetHistoryAsync(long chatId, int count = 5);
        Task<ExchangeRateHistory?> GetLastAsync(long chatId);
    }
}