using CurrencyExchangeBot.App.Data;
using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Services
{
    public class HistoryRepository(AppDbContext db) : IHistoryRepository
    {
        public async Task SaveBatchAsync(IEnumerable<HistoryEntry> entries, CancellationToken ct)
        {
            var entities = entries.Select(e => new ExchangeRateHistory
            {
                ChatId = e.ChatId,
                Currency = e.Currency,
                Date = e.Date,
                PurchaseRate = e.PurchaseRate,
                SaleRate = e.SaleRate,
            }).ToList();

            await db.ExchangeRateHistories.AddRangeAsync(entities, ct);
            await db.SaveChangesAsync(ct);
        }
    }
}
