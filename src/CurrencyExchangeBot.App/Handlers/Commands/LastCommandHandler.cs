using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers.Commands
{
    public class LastCommandHandler(
        ITelegramBotClient bot,
        IRateMessageFormatter formatter,
        IServiceScopeFactory scopeFactory)
        : ICommandHandler, ICallBackHandler
    {
        public string Command => "/last";
        public string CallbackData => "last";

        public bool CanHandle(string data) => data == CallbackData;

        public async Task HandleAsync(long chatId, string message, CancellationToken ct)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var historyService = scope.ServiceProvider.GetRequiredService<IHistoryReader>();
            var last = await historyService.GetLastAsync(chatId);

            if (last is null)
            {
                await bot.SendMessage(chatId, "Ще немає жодного запиту.", cancellationToken: ct);
                return;
            }

            await bot.SendMessage(chatId,
                formatter.FormatLastEntry(last),
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}