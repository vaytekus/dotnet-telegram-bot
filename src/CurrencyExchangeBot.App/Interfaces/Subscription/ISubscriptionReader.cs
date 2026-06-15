using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface ISubscriptionReader
    {
        Task<UserSubscription?> GetSubscriptionAsync(long chatId);
        Task<List<UserSubscription>> GetDueSubscriptionsAsync(TimeOnly currentTime);
    }
}