using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IHistoryQueueReader
    {
        IAsyncEnumerable<HistoryEntry[]> ReadBatchesAsync(CancellationToken ct);
    }
}