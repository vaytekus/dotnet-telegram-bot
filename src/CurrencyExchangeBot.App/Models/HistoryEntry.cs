namespace CurrencyExchangeBot.App.Models
{
    public record HistoryEntry(
        long ChatId,
        string Currency,
        DateTime Date,
        decimal PurchaseRate,
        decimal SaleRate);
}