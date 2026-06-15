using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Services
{
    public class NotificationSender(
        INotificationDataProvider dataProvider,
        IRateMessageFormatter formatter,
        ITelegramBotClient bot,
        ILogger<NotificationSender> logger) : INotificationSender
    {
        public async Task SendAsync(CancellationToken ct)
        {
            var currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);
            var notifications = await dataProvider.GetDueNotificationsAsync(currentTime, ct);

            foreach (var n in notifications)
            {
                try
                {
                    await bot.SendMessage(n.ChatId,
                        formatter.FormatRate(n.Currency, DateTime.UtcNow.Date, n.PurchaseRate, n.SaleRate),
                        parseMode: ParseMode.Markdown,
                        cancellationToken: ct);

                    logger.LogInformation("Notification sent to {ChatId} for {Currency}", n.ChatId, n.Currency);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to send notification to {ChatId}", n.ChatId);
                }
            }
        }
    }
}