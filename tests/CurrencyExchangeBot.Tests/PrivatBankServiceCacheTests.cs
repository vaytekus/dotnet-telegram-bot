using System.Net;
using System.Text;
using CurrencyExchangeBot.App.Services;
using CurrencyExchangeBot.App.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace CurrencyExchangeBot.Tests;

public class PrivatBankServiceCacheTests
{
    [Fact]
    public async Task GetRateAsync_1000ConcurrentRequests_CallsApiOnce()
    {
        // Arrange
        var callCount = 0;
        var handler = new MockHttpMessageHandler(request =>
        {
            Interlocked.Increment(ref callCount);
            var json = """
               {
                   "exchangeRate": [
                        { "currency": "USD", "saleRate": 39.5, "purchaseRate": 38.5 }
                   ]
               }
               """;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        });

        var httpClient = new HttpClient(handler);
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new PrivatBankSettings { ApiUrl = "https://fake-api.com/" });
        var logger = NullLogger<PrivatBankService>.Instance;

        var service = new PrivatBankService(httpClient, options, cache, logger);

        var tasks = Enumerable.Range(0, 1000)
            .Select(_ => service.GetRateAsync("USD", DateTime.Today, CancellationToken.None).AsTask());

        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(1, callCount);
        Assert.All(results, r => Assert.True(r.IsSuccess));
        Assert.All(results, r => Assert.Equal("USD", r.Value!.Currency));
    }
}
