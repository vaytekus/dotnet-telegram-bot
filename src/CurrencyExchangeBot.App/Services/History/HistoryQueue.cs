using System.Runtime.CompilerServices;
using System.Threading.Channels;
using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeBot.App.Services
{
    public class HistoryQueue(ILogger<HistoryQueue> logger) : IHistoryQueue
    {
        private const int _capacity = 1000;
        private readonly Channel<HistoryEntry> _channel =
            Channel.CreateBounded<HistoryEntry>(_capacity);

        public void Enqueue(HistoryEntry entry)
        {
            if (!_channel.Writer.TryWrite(entry))
            {
                logger.LogWarning("History queue full, entry dropped for {ChatId}", entry.ChatId);
            }
        }

        public async IAsyncEnumerable<HistoryEntry[]> ReadBatchesAsync(
            [EnumeratorCancellation] CancellationToken ct)
        {
            while (await _channel.Reader.WaitToReadAsync(ct))
            {
                var batch = new List<HistoryEntry>();

                while (_channel.Reader.TryRead(out HistoryEntry? entry) && entry is not null)
                {
                    batch.Add(entry);
                }
                
                yield return batch.ToArray();
            }
        }
    }
}