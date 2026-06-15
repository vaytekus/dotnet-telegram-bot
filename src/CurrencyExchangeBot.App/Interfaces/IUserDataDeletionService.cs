namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IUserDataDeletionService
    {
        Task DeleteAllAsync(long chatId, CancellationToken ct);
    }
}