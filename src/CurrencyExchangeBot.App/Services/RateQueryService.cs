using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Services
{
    public class RateQueryService(
        IServiceScopeFactory scopeFactory,
        ITelegramBotClient bot,
        IDateValidator dateValidator,
        IRateMessageFormatter formatter,
        IPrivatBankService privatBank) : IRateQueryService
    {
        public async Task<bool> TryHandleAsync(long chatId, string currency, string dateInput, CancellationToken ct)
        {
            if (!dateValidator.TryValidate(dateInput, out var date, out _))
            {
                return false;
            }
            
            var result = await privatBank.GetRateAsync(currency, date, ct);

            return await result.Match(
                onSuccess: async rate =>
                {
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var historyWriter = scope.ServiceProvider.GetRequiredService<IHistoryWriter>();
                    await historyWriter.SaveAsync(chatId, currency, date, rate.PurchaseRate, rate.SaleRate);
                    
                    await bot.SendMessage(chatId,
                        formatter.FormatRate(currency, date, rate.PurchaseRate, rate.SaleRate),
                        parseMode: ParseMode.Markdown, cancellationToken: ct);
                    
                    return true;
                },
                onNotFound: async msg =>
                {
                    await bot.SendMessage(chatId, msg, cancellationToken: ct);
                    return true;
                },
                onFailure: async msg =>
                {
                    await bot.SendMessage(chatId, msg, cancellationToken: ct);
                    return false;
                }
            );
        }
    }
}