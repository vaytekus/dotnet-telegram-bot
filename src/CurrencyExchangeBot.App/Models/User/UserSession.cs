using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangeBot.App.Models
{
    public class UserSession
    {
        [Key]
        public long ChatId { get; set; }
        public string? SelectedCurrency { get; set; }
        public UserState State { get; set; } = UserState.Start;
    }
}