using Microsoft.Extensions.Configuration;

namespace CurrencyExchangeBot.App.Helper
{
    public class ConfigurationHelper
    {
        public const string DefaultConnectionName = "DefaultConnection";

        public static IConfiguration Build()
        {
            string? env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}