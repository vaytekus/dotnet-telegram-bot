namespace CurrencyExchangeBot.App.Models
{
    public class ExchangeRateResponse
    {
        public string? Date { get; set; }
        public List<ExchangeRate> ExchangeRate { get; set; } = [];
    }
}