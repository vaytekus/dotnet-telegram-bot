using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangeBot.App.Models
{
    public class UserSubscription
    {
        [Key]
        public long ChatId { get; set; }
        public required string Currency { get; set; }
        public TimeOnly NotificationTime { get; set; }
    }
}