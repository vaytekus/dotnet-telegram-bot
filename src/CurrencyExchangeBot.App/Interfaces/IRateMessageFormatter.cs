using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IRateMessageFormatter
    {
        string FormatRate(string currency, DateTime time, decimal purchaseRate, decimal saleRate);
        string FormatHistoryEntry(ExchangeRateHistory entry);
        string FormatLastEntry(ExchangeRateHistory entry);
    }
}