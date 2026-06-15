namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IHistoryWriter
    {
        Task SaveAsync(long chatId, string currency, DateTime date, decimal purchaseRate, decimal saleRate);
    }
}