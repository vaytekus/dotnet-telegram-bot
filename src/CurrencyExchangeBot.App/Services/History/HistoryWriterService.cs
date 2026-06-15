using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeBot.App.Services
{
    public class HistoryWriterService(
        IHistoryQueueReader queue,
        IServiceScopeFactory scopeFactory,
        ILogger<HistoryWriterService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var batch in queue.ReadBatchesAsync(stoppingToken))
            {
                try
                {
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IHistoryRepository>();
                    await repository.SaveBatchAsync(batch, stoppingToken);
                    logger.LogInformation("Saved batch of {Count} history entries", batch.Length);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to save history batch");
                }
            }
        }
    }
}