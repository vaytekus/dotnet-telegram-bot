namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IHelpService
    {
        Task SendHelpAsync(long chatId, CancellationToken ct);
    }
}