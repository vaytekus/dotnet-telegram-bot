namespace CurrencyExchangeBot.App.Interfaces
{
    public interface IDateValidator
    {
        bool TryValidate(string input, out DateTime date, out string errorMessage);
    }
}