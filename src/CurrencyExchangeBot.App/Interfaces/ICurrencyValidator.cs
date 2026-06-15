namespace CurrencyExchangeBot.App.Interfaces
{
    public interface ICurrencyValidator
    {
        public bool TryValidate(string input, out string currency, out string errorMessage);
    }
}