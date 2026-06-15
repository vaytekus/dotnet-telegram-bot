using CurrencyExchangeBot.App.Interfaces;

namespace CurrencyExchangeBot.App.Services
{
    public class DateValidator : IDateValidator
    {
        private const string _dateFormat = "dd.MM.yyyy";
        private const int _maxHistoryYears = -3;

        public bool TryValidate(string input, out DateTime date, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be empty.", nameof(input));
            }

            errorMessage = string.Empty;
            if (!DateTime.TryParseExact(input, _dateFormat, null, System.Globalization.DateTimeStyles.None, out date))
            {
                errorMessage = $"Невірний формат дати. Використовуйте `{_dateFormat}`";
                return false;
            }

            if (date > DateTime.Today)
            {
                errorMessage = "Не можна вводити майбутні дати.";
                return false;
            }

            if (date < DateTime.Today.AddYears(_maxHistoryYears))
            {
                errorMessage = $"Дата занадто стара. Введіть дату не старіше {-_maxHistoryYears} років.";
                return false;
            }

            return true;
        }
    }
}