namespace CurrencyExchangeBot.App.Interfaces
{
    public interface ICommandHandler
    {
        string Command { get; }
        Task HandleAsync(long chatId, string text, CancellationToken ct);
    }
}