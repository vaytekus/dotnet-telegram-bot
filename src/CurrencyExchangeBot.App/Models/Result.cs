namespace CurrencyExchangeBot.App.Models
{
    public class Result<T>
    {
        public T? Value { get; }
        public string? Error { get; }
        public bool IsSuccess { get; }
        public bool IsNotFound { get; }

        private Result(T value)
        {
            Value = value;
            IsSuccess = true;
        }
        
        private Result(string error, bool isNotFound)
        {
            Error = error;
            IsNotFound = isNotFound;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> NotFound(string error) => new(error, isNotFound: true);
        public static Result<T> Failure(string error) => new(error, isNotFound: false);
        
        public TResult Match<TResult>(
            Func<T, TResult> onSuccess,
            Func<string, TResult> onNotFound,
            Func<string, TResult> onFailure) => IsSuccess 
            ? onSuccess(Value!) 
            : IsNotFound 
                ? onNotFound(Error!) 
                : onFailure(Error!);

        public void Switch(
            Action<T> onSuccess,
            Action<string> onNotFound,
            Action<string> onFailure)
        {
            if (IsSuccess)
            {
                onSuccess(Value!);
            }
            else if (IsNotFound)
            {
                onNotFound(Error!);
            }
            else
            {
                onFailure(Error!);
            }
        }
    }
}