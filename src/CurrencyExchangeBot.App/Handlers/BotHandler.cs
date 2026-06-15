using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Handlers
{
    public class BotHandler(
        ITelegramBotClient bot,
        ILogger<BotHandler> logger,
        IBotErrorHandler errorHandler,
        IServiceScopeFactory scopeFactory,
        IEnumerable<ICommandHandler> commandHandlers,
        IEnumerable<ICallBackHandler> callBackHandlers,
        IEnumerable<IStateHandler> stateHandlers)
    {
        private readonly Dictionary<string, ICommandHandler> _commands = 
            commandHandlers.ToDictionary(x => x.Command);

        private readonly Dictionary<UserState, IStateHandler> _stateHandlers =
            stateHandlers.ToDictionary(h => h.State);
        
        public async Task HandleUpdateAsync(Update update, CancellationToken ct)
        {
            try
            {
                if (update.Message?.Text is { } text)
                {
                    await HandleMessageAsync(update.Message, text, ct);
                }
                else if (update.CallbackQuery is { } callbackQuery)
                {
                    await HandleCallbackAsync(callbackQuery, ct);
                }
            }
            catch (Exception ex)
            {
                await errorHandler.HandleAsync(update, ex, ct);
            }
        }

        private async Task HandleMessageAsync(Message message, string text, CancellationToken ct)
        {
            var chatId = message.Chat.Id;
            logger.LogInformation("Message from {ChatId}: {Text}", chatId, text);

            if (_commands.TryGetValue(text.Trim(), out var handler))
            {
                await handler.HandleAsync(chatId, text, ct);
                return;
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var sessionReader = scope.ServiceProvider.GetRequiredService<ISessionReader>();
            var session = await sessionReader.GetAsync(chatId);

            if (session is not null && _stateHandlers.TryGetValue(session.State, out var stateHandler))
            {
                await stateHandler.HandleAsync(chatId, text, ct);
                return;
            }

            logger.LogInformation("No active session for {ChatId}, showing currency keyboard", chatId);
            await bot.SendMessage(chatId,
                "Введіть код валюти (наприклад: `USD`, `EUR`, `GBP`):",
                parseMode: ParseMode.Markdown, cancellationToken: ct); 
        }

        private async Task HandleCallbackAsync(CallbackQuery callbackQuery, CancellationToken ct)
        {
            if (callbackQuery.Message is null) return;

            var chatId = callbackQuery.Message.Chat.Id;
            var data = callbackQuery.Data;

            if (data is null) return;

            await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);

            var handler = callBackHandlers.FirstOrDefault(h => h.CanHandle(data));

            if (handler is not null)
            {
                await handler.HandleAsync(chatId, data, ct);
                return;
            }
        }
    }
}