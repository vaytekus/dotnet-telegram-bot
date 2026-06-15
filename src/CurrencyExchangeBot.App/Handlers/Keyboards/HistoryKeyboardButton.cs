using CurrencyExchangeBot.App.Interfaces;

namespace CurrencyExchangeBot.App.Handlers.Keyboards
{
    public class HistoryKeyboardButton : IKeyboardButton
    {
        public string ButtonText => "📋 Історія";
        public string CallbackData => "history";
    }
}