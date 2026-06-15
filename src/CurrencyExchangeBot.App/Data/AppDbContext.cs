using CurrencyExchangeBot.App.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeBot.App.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
        public DbSet<ExchangeRateHistory> ExchangeRateHistories => Set<ExchangeRateHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSession>()
                .Property(s => s.ChatId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<UserSubscription>()
                .Property(s => s.ChatId)
                .ValueGeneratedNever();

            modelBuilder.Entity<ExchangeRateHistory>(b =>
            {
                b.Property(h => h.PurchaseRate).HasPrecision(18, 4);
                b.Property(h => h.SaleRate).HasPrecision(18, 4);
            });
        }
    }
}