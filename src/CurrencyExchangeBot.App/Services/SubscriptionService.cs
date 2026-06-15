using CurrencyExchangeBot.App.Data;
using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeBot.App.Services
{
    public class SubscriptionService(AppDbContext db) : ISubscriptionService
    {
        public async Task SubscribeAsync(long chatId, string currency, TimeOnly time)
        {
            var sub = await db.UserSubscriptions.FindAsync(chatId);
            
            if (sub is null)
            {
                sub = new UserSubscription { ChatId = chatId, Currency = currency };
                db.UserSubscriptions.Add(sub);
            }
            else
            {
                sub.Currency = currency;
            }
            sub.NotificationTime = time;
            await db.SaveChangesAsync();
        }
        
        public async Task<UserSubscription?> GetSubscriptionAsync(long chatId)
            => await db.UserSubscriptions.FindAsync(chatId);

        public async Task UnsubscribeAsync(long chatId)
            => await db.UserSubscriptions
                .Where(u => u.ChatId == chatId)
                .ExecuteDeleteAsync();

        public async Task<List<UserSubscription>> GetDueSubscriptionsAsync(TimeOnly currentTime)
            => await db.UserSubscriptions
                .Where(s => s.NotificationTime.Hour == currentTime.Hour
                            && s.NotificationTime.Minute == currentTime.Minute)
                .ToListAsync();
    }
}