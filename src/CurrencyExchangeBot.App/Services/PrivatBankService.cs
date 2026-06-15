using System.Net.Http.Json;
using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using CurrencyExchangeBot.App.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CurrencyExchangeBot.App.Services
{
    public class PrivatBankService(
        HttpClient httpClient,
        IOptions<PrivatBankSettings> options,
        
        IMemoryCache cache,
        ILogger<PrivatBankService> logger) : IPrivatBankService
    {
        private const string _dateFormat = "dd.MM.yyyy";
        private const int _cacheTtlMinutes = 10;
        private const string _cacheKeyPrefix = "rate_";
        private static readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(_cacheTtlMinutes);
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly PrivatBankSettings _settings = options.Value;

        public Task<Result<ExchangeRate>> GetRateAsync(string currency, DateTime date, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency cannot be empty.", nameof(currency));
            }

            var cacheKey = $"{_cacheKeyPrefix}{currency}_{date:yyyyMMdd}";

            if (cache.TryGetValue(cacheKey, out ExchangeRate? cached))
            {
                logger.LogInformation("Getting rate {Currency} from cache", currency);
                return Task.FromResult(cached is not null 
                    ? Result<ExchangeRate>.Success(cached)
                    : Result<ExchangeRate>.NotFound($"Дані для {currency} на {date:dd.MM.yyyy} не знайдено."));
            }

            return FetchAndCacheAsync(currency, date, cacheKey, ct);
        }

        private async Task<Result<ExchangeRate>> FetchAndCacheAsync(string currency, DateTime date, string cacheKey, CancellationToken ct)
        {
            await _semaphore.WaitAsync(ct);
            
            try
            {
                if (cache.TryGetValue(cacheKey, out ExchangeRate? cached))
                {
                    logger.LogInformation("Getting rate {Currency} from cache", currency);
                    return cached is not null
                        ? Result<ExchangeRate>.Success(cached)
                        : Result<ExchangeRate>.NotFound($"Дані для {currency} на {date:dd.MM.yyyy} не знайдено.");
                }
                
                var dateString = date.ToString(_dateFormat);
                var url = _settings.ApiUrl + dateString;
                logger.LogInformation("Fetching rate for {Currency} on {Date} from {Url}", currency, dateString, url);

                var response = await httpClient.GetFromJsonAsync<ExchangeRateResponse>(url, ct);

                var rate = response?.ExchangeRate
                    .FirstOrDefault(r => r.Currency?.Equals(currency, StringComparison.OrdinalIgnoreCase) == true);

                cache.Set(cacheKey, rate, _cacheTtl);
                return rate is not null
                    ? Result<ExchangeRate>.Success(rate)
                    : Result<ExchangeRate>.NotFound($"Дані для {currency} на {date:dd.MM.yyyy} не знайдено.");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}