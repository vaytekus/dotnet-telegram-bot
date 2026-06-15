using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface INotificationDataProvider
    {
        Task<IEnumerable<NotificationDataModel>> GetDueNotificationsAsync(TimeOnly currentTime, CancellationToken ct);
    }
}