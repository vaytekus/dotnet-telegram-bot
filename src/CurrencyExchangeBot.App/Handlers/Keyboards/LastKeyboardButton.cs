using CurrencyExchangeBot.App.Interfaces;

namespace CurrencyExchangeBot.App.Handlers.Keyboards
{
    public class LastKeyboardButton : IKeyboardButton
    {
        public string ButtonText => "🕐 Останній";
        public string CallbackData => "last";
    }
}