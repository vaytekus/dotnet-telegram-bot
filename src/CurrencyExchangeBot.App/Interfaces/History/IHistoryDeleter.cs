namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IHistoryDeleter
    {
        Task DeleteAllAsync(long chatId);
    }
}