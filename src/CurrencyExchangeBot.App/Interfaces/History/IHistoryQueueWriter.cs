using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IHistoryQueueWriter
    {
        void Enqueue(HistoryEntry entry);
    }
}