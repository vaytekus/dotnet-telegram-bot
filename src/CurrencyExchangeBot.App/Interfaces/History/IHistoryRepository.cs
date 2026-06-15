using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IHistoryRepository
    {
        Task SaveBatchAsync(IEnumerable<HistoryEntry> entries, CancellationToken ct);
    }
}
