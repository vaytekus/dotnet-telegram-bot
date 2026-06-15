namespace CurrencyExchangeBot.App.Models
{
    public enum UserState
    {
        Start,
        WaitingForCurrency,
        WaitingForDate,
        WaitingForRateCurrency,
        WaitingForSubscribeCurrency,
        WaitingForSubscribeTime
    }
}