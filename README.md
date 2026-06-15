# dotnet-telegram-bot

Telegram bot for querying currency exchange rates from PrivatBank API with scheduled notifications and request history.

## Features

- Get current or historical exchange rates by currency and date
- Subscribe to daily rate notifications at a chosen time
- View request history with `/history`
- Get last queried rate with `/last`
- Unsubscribe from notifications with `/unsubscribe`
- Delete all personal data with `/deletedata`
- In-memory caching with cache stampede protection

## Stack

- .NET 10 / C#
- Telegram.Bot 22
- Entity Framework Core + SQL Server
- Microsoft.Extensions.Http.Resilience (Polly)
- Serilog

## Database

Start SQL Server via Docker:

```bash
docker-compose up -d
```

## Configuration

Copy the example config and fill in your values:

```bash
cp src/CurrencyExchangeBot.App/appsettings.example.json src/CurrencyExchangeBot.App/appsettings.Development.json
```

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CurrencyBotDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "BotToken": "YOUR_TELEGRAM_BOT_TOKEN",
  "PrivatBank": {
    "ApiUrl": "https://api.privatbank.ua/p24api/exchange_rates?json&date="
  }
}
```

`appsettings.Development.json` is excluded from git. Logs are written to `src/Logs/log-YYYYMMDD.txt`, a new file is created each day.

## Run

```bash
dotnet run --project src/CurrencyExchangeBot.App
```
