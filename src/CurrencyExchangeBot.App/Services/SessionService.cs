using CurrencyExchangeBot.App.Data;
using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeBot.App.Services
{
    public class SessionService(AppDbContext dbContext) : ISessionService
    {
        public async Task<UserSession?> GetAsync(long chatId) 
            => await dbContext.UserSessions.FindAsync(chatId);

        public async Task SaveCurrencyAsync(long chatId, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency cannot be empty.", nameof(currency));
            }

            var session = await GetOrCreateSessionAsync(chatId);
            
            session.SelectedCurrency = currency;
            await dbContext.SaveChangesAsync();
        }
        
        public async Task ResetAsync(long chatId) 
            => await dbContext.UserSessions
                .Where(s => s.ChatId == chatId)
                .ExecuteDeleteAsync();

        public async Task SaveStateAsync(long chatId, UserState state)
        {
            var session = await GetOrCreateSessionAsync(chatId);
            session.State = state;
            await dbContext.SaveChangesAsync();
        }

        private async Task<UserSession> GetOrCreateSessionAsync(long chatId)
        {
            var session = await dbContext.UserSessions.FindAsync(chatId);
            if (session is null)
            {
                session = new UserSession { ChatId = chatId };
                dbContext.UserSessions.Add(session);
            }
            return session;
        }
    }
}