using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeBot.App.Services
{
    public class NotificationService(
        INotificationSender sender,
        ILogger<NotificationService> logger) :
        BackgroundService
    {
        private const int _checkIntervalMinutes = 1;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("NotificationService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                await sender.SendAsync(stoppingToken);
                var now = DateTime.UtcNow;
                var nextMinute = now.AddMinutes(_checkIntervalMinutes).AddSeconds(-now.Second);
                await Task.Delay(nextMinute - now, stoppingToken);
            }
        }
    }
}