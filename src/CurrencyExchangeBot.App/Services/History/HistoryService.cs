using CurrencyExchangeBot.App.Data;
using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeBot.App.Services
{
    public class HistoryService(IHistoryQueueWriter queue, AppDbContext db) : IHistoryService
    {
        public Task SaveAsync(long chatId, string currency, DateTime date, decimal purchaseRate, decimal saleRate)
        {
            queue.Enqueue(new HistoryEntry(chatId, currency, date, purchaseRate, saleRate));
            return Task.CompletedTask;
        }

        public async Task<List<ExchangeRateHistory>> GetHistoryAsync(long chatId, int count = 5)
            => await db.ExchangeRateHistories
                .Where(h => h.ChatId == chatId)
                .OrderByDescending(h => h.RequestedAt)
                .Take(count)
                .ToListAsync();

        public async Task<ExchangeRateHistory?> GetLastAsync(long chatId)
            => await db.ExchangeRateHistories
                .Where(h => h.ChatId == chatId)
                .OrderByDescending(h => h.RequestedAt)
                .FirstOrDefaultAsync();

        public async Task DeleteAllAsync(long chatId)
            => await db.ExchangeRateHistories
                .Where(h => h.ChatId == chatId)
                .ExecuteDeleteAsync();
    }
}