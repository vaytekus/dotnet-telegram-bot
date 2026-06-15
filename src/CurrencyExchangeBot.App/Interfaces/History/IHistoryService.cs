namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IHistoryService : IHistoryWriter, IHistoryReader, IHistoryDeleter
    {
    }
}