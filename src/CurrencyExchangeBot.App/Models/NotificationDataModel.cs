namespace CurrencyExchangeBot.App.Models
{
    public record NotificationDataModel(
        long ChatId, 
        string Currency, 
        decimal PurchaseRate, 
        decimal SaleRate);
}