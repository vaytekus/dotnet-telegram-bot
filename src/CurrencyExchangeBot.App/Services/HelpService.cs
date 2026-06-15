using CurrencyExchangeBot.App.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Services
{
    public class HelpService(ITelegramBotClient bot) : IHelpService
    {
        public async Task SendHelpAsync(long chatId, CancellationToken ct)
        {
            await bot.SendMessage(chatId,
                "*Як користуватись ботом:*\n\n" +
                "1. Введіть код валюти або валюту з датою одразу:\n" +
                "   `USD` — крок за кроком\n" +
                "   `USD 01.06.2024` — одразу отримати курс\n" +
                "2. Для підписки: `USD` або `USD 08:00`\n\n" +
                "*Команди:*\n" +
                "/start — головне меню\n" +
                "/rate — поточний курс USD, EUR, GBP\n" +
                "/history — останні 5 запитів\n" +
                "/last — останній запит\n" +
                "/subscribe — підписатись на щоденний курс\n" +
                "/unsubscribe — відписатись від сповіщень\n" +
                "/deletedata — видалити всі мої дані\n" +
                "/help — ця інструкція",
                ParseMode.Markdown, cancellationToken: ct);
        }
    }
}