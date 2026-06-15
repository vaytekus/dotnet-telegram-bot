namespace CurrencyExchangeBot.App.Interfaces
{
    public interface ICallBackHandler
    {
        bool CanHandle(string data);
        Task HandleAsync(long chatId, string text, CancellationToken ct);
    }
}