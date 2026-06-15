namespace CurrencyExchangeBot.App.Interfaces
{
    public interface ISubscriptionWriter
    {
        Task SubscribeAsync(long chatId, string currency, TimeOnly time);
        Task UnsubscribeAsync(long chatId);
    }
}