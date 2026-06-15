using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;

namespace CurrencyExchangeBot.App.Services
{
    public class RateMessageFormatter : IRateMessageFormatter
    {
        private const string _dateFormat = "dd.MM.yyyy";
        
        public string FormatRate(string currency, DateTime date, decimal purchaseRate, decimal saleRate) =>
            $"*{currency}/UAH на {date.ToString(_dateFormat)}:*\n" +
            $"Купівля: `{purchaseRate}`\n" +
            $"Продаж: `{saleRate}`";

        public string FormatHistoryEntry(ExchangeRateHistory entry) =>
            $"`{entry.Date:dd.MM.yyyy}` — *{entry.Currency}/UAH* купівля: `{entry.PurchaseRate}` продаж: `{entry.SaleRate}`";
        
        public string FormatLastEntry(ExchangeRateHistory entry) =>
            $"*Останній запит:*\n" +
            $"`{entry.Date:dd.MM.yyyy}` — *{entry.Currency}/UAH*\n" +
            $"Купівля: `{entry.PurchaseRate}`\n" +
            $"Продаж: `{entry.SaleRate}`";
    }
}