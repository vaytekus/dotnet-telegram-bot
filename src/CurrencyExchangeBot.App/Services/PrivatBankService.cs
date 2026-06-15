using System.Collections.Concurrent;
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
        private readonly PrivatBankSettings _settings = options.Value;
        
        private readonly ConcurrentDictionary<string, Lazy<Task<Result<ExchangeRate>>>> _inFlight = new ();

        public ValueTask<Result<ExchangeRate>> GetRateAsync(string currency, DateTime date, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency cannot be empty.", nameof(currency));
            }

            var cacheKey = $"{_cacheKeyPrefix}{currency}_{date:yyyyMMdd}";

            if (cache.TryGetValue(cacheKey, out ExchangeRate? cached))
            {
                logger.LogInformation("Getting rate {Currency} from cache", currency);
                return ValueTask.FromResult(cached is not null 
                    ? Result<ExchangeRate>.Success(cached)
                    : Result<ExchangeRate>.NotFound($"Дані для {currency} на {date:dd.MM.yyyy} не знайдено."));
            }

            var lazy = _inFlight.GetOrAdd(cacheKey,
                _ => new Lazy<Task<Result<ExchangeRate>>>(() => FetchAndCacheAsync(currency, date, cacheKey, ct)));

            return new ValueTask<Result<ExchangeRate>>(FetchWithCleanupAsync(cacheKey, lazy));
        }

        private async Task<Result<ExchangeRate>> FetchWithCleanupAsync(string cacheKey,
            Lazy<Task<Result<ExchangeRate>>> lazy)
        {
            try
            {
                return  await lazy.Value;
            }
            finally
            {
                _inFlight.TryRemove(cacheKey, out _);
            }
        }

        private async Task<Result<ExchangeRate>> FetchAndCacheAsync(string currency, DateTime date, string cacheKey, CancellationToken ct)
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
    }
}