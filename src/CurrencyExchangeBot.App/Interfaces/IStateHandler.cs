using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IStateHandler
    {
        UserState State { get; }
        Task HandleAsync(long chatId, string text, CancellationToken ct);
    }
}