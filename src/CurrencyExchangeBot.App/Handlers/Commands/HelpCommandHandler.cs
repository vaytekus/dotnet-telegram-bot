using CurrencyExchangeBot.App.Interfaces;

namespace CurrencyExchangeBot.App.Handlers.Commands
{
    public class HelpCommandHandler(IHelpService helpService) : ICommandHandler
    {
        public string Command => "/help";

        public async Task HandleAsync(long chatId, string text, CancellationToken ct)
        {
            await helpService.SendHelpAsync(chatId, ct);
        }
    }
}