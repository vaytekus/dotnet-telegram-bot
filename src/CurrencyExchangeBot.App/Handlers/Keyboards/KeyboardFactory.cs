using CurrencyExchangeBot.App.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;
using IKeyboardButton = CurrencyExchangeBot.App.Interfaces.IKeyboardButton;

namespace CurrencyExchangeBot.App.Handlers.Keyboards
{
    public class KeyboardFactory(IEnumerable<IKeyboardButton> buttons) : IKeyboardFactory
    {
        public InlineKeyboardMarkup CurrencyKeyboard() => new(
        [
            buttons.Select(b => InlineKeyboardButton.WithCallbackData(b.ButtonText, b.CallbackData)).ToArray()
        ]);
    }
}