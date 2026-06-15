using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeBot.App.Services
{
    public class UserDataDeletionService(
        IServiceScopeFactory scopeFactory,
        ILogger<UserDataDeletionService> logger) : IUserDataDeletionService
    {
        public async Task DeleteAllAsync(long chatId, CancellationToken ct)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var session = scope.ServiceProvider.GetRequiredService<ISessionWriter>();
            var history = scope.ServiceProvider.GetRequiredService<IHistoryDeleter>();
            var subscription = scope.ServiceProvider.GetRequiredService<ISubscriptionWriter>();
            
            await session.ResetAsync(chatId);
            await subscription.UnsubscribeAsync(chatId);
            await history.DeleteAllAsync(chatId);

            logger.LogInformation("All data deleted for {ChatId}", chatId);
        }
    }
}