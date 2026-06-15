namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IKeyboardButton
    {
        string ButtonText { get; }
        string CallbackData { get; }
    }
}