using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CurrencyExchangeBot.App.Handlers.Commands
{
    public class DeleteDataCommandHandler(
        ITelegramBotClient bot,
        IUserDataDeletionService deletionService,
        ILogger<DeleteDataCommandHandler> logger) : ICommandHandler
    {
        public string Command => "/deletedata";

        public async Task HandleAsync(long chatId, string text, CancellationToken ct)
        {
            try
            {
                await deletionService.DeleteAllAsync(chatId, ct);

                await bot.SendMessage(chatId,
                    "Всі ваші дані видалено: сесія, підписка та історія запитів.",
                    cancellationToken: ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to delete data for {ChatId}", chatId);
                await bot.SendMessage(chatId,
                    "Не вдалось видалити дані. Спробуйте пізніше.",
                    cancellationToken: ct); 
            }
        }
    }
}