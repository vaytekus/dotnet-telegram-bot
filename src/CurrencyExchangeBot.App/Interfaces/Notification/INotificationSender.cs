namespace CurrencyExchangeBot.App.Interfaces
{
    public interface INotificationSender
    {
        Task SendAsync(CancellationToken ct);
    }
}