using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface ISessionReader
    {
        Task<UserSession?> GetAsync(long chatId);
    }
}
