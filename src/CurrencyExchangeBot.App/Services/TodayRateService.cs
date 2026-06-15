using CurrencyExchangeBot.App.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App.Services
{
    public class TodayRateService(
        ITelegramBotClient bot,
        IPrivatBankService privatBank,
        IRateMessageFormatter formatter,
        ILogger<TodayRateService> logger) : ITodayRateService
    {
        public async Task<bool> HandleAsync(long chatId, string currency, CancellationToken ct)
        {
            var today = DateTime.UtcNow.Date;
            var result = await privatBank.GetRateAsync(currency, today, ct);

            return await result.Match(
                onSuccess: async rate =>
                {
                    logger.LogInformation("Rate sent to {ChatId}: {Currency} buy={Buy} sale={Sale}",
                        chatId, currency, rate.PurchaseRate, rate.SaleRate);
                    
                    await bot.SendMessage(chatId,
                        formatter.FormatRate(currency, today, rate.PurchaseRate, rate.SaleRate),
                        parseMode: ParseMode.Markdown, cancellationToken: ct);
                    
                    return true;
                },
                onNotFound: async msg =>
                {
                    logger.LogWarning("Rate not found for {Currency} on {Date}", currency, today);
                    await bot.SendMessage(chatId, msg, parseMode: ParseMode.Markdown, cancellationToken: ct);
                    return false; 
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