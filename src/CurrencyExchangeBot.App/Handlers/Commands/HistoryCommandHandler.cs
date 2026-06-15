using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers.Commands
{
    public class HistoryCommandHandler(
        ITelegramBotClient bot,
        IRateMessageFormatter formatter,
        IServiceScopeFactory scopeFactory)
        : ICommandHandler, ICallBackHandler
    {
        public string Command => "/history";
        public string CallbackData => "history";

        public bool CanHandle(string data) => data == CallbackData;

        public async Task HandleAsync(long chatId, string message, CancellationToken ct)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var historyService = scope.ServiceProvider.GetRequiredService<IHistoryReader>();
            var history = await historyService.GetHistoryAsync(chatId);

            if (!history.Any())
            {
                await bot.SendMessage(chatId, "Історія запитів порожня.", cancellationToken: ct);
                return;
            }

            var lines = history.Select(h => formatter.FormatHistoryEntry(h)).ToArray();

            await bot.SendMessage(chatId,
                $"*Останні запити:*\n{string.Join("\n", lines)}",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}