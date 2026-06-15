using Telegram.Bot.Types.ReplyMarkups;

namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IKeyboardFactory
    {
        InlineKeyboardMarkup CurrencyKeyboard();
    }
}