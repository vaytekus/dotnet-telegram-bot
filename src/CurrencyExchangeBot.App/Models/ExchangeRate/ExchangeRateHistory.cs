namespace CurrencyExchangeBot.App.Models
{
    public class ExchangeRateHistory
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public decimal PurchaseRate { get; set; }
        public decimal SaleRate { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}