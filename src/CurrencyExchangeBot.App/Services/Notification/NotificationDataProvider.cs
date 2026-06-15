using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeBot.App.Services
{
    public class NotificationDataProvider(
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationDataProvider> logger) : INotificationDataProvider
    {
        public async Task<IEnumerable<NotificationDataModel>> GetDueNotificationsAsync(TimeOnly currentTime, CancellationToken ct)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionReader>();
            var privatBank = scope.ServiceProvider.GetRequiredService<IPrivatBankService>();

            var subscriptions = await subscriptionService.GetDueSubscriptionsAsync(currentTime);
            var result = new List<NotificationDataModel>();

            foreach (var sub in subscriptions)
            {
                try
                {
                    var rateResult = await privatBank.GetRateAsync(sub.Currency, DateTime.UtcNow.Date, ct);

                    rateResult.Switch(
                        onSuccess: rate => result.Add(new NotificationDataModel(
                            sub.ChatId, sub.Currency, rate.PurchaseRate, rate.SaleRate)),
                        onNotFound: _ => logger.LogWarning("Rate not found for {Currency}", sub.Currency),
                        onFailure: _ => logger.LogWarning("Rate fetch failed for {Currency}", sub.Currency)
                        );
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to fetch rate for {Currency}", sub.Currency);
                }
            }

            return result;
        }
    }
}