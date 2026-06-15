using CurrencyExchangeBot.App.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CurrencyExchangeBot.App.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = ConfigurationHelper.Build();
            var connectionString = config.GetConnectionString(ConfigurationHelper.DefaultConnectionName);
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}