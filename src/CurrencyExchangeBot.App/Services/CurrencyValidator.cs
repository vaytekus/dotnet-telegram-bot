using CurrencyExchangeBot.App.Interfaces;

namespace CurrencyExchangeBot.App.Services
{
    public class CurrencyValidator : ICurrencyValidator
    {
        private const int _minLength = 2;
        private const int _maxLength = 5;
        
        public bool TryValidate(string input, out string currency, out string errorMessage)
        {
            currency = input.Trim().ToUpperInvariant();
            if (currency.Length < _minLength || currency.Length > _maxLength || !currency.All(char.IsLetter))
            {
                errorMessage = "Невірний формат. Введіть код валюти латинськими літерами (наприклад: `USD`, `EUR`, `CHF`):";
                return false;
            }
            
            errorMessage = string.Empty;
            return true;
        }
    }
}