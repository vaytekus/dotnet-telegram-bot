namespace CurrencyExchangeBot.App.Models
{
    public class ExchangeRate
    {
        public string? Currency { get; set; }
        public decimal SaleRate { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal SaleRateNB { get; set; }
        public decimal PurchaseRateNB { get; set; }
    }
}