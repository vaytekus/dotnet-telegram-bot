using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface ISessionWriter
    {
        Task SaveCurrencyAsync(long chatId, string currency);
        Task SaveStateAsync(long chatId, UserState state);
        Task ResetAsync(long chatId);
    }
}
