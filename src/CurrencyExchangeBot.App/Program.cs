using CurrencyExchangeBot.App.Data;
using CurrencyExchangeBot.App.Handlers;
using CurrencyExchangeBot.App.Handlers.Commands;
using CurrencyExchangeBot.App.Handlers.Exceptions;
using CurrencyExchangeBot.App.Handlers.Keyboards;
using CurrencyExchangeBot.App.Helper;
using CurrencyExchangeBot.App.Interfaces;
using CurrencyExchangeBot.App.Services;
using CurrencyExchangeBot.App.Settings;
using Microsoft.EntityFrameworkCore;
using Polly;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CurrencyExchangeBot.App;

internal class Program
{
    private const string _logsFolder = "Logs";
    private const string _logFileName = "log-.txt";
    private const int _retainedLogFileCount = 10;

    private static async Task Main(string[] args)
    {
        var config = ConfigurationHelper.Build();
        var logPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", _logsFolder, _logFileName);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: _retainedLogFileCount,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();

        string connectionString = config.GetConnectionString(ConfigurationHelper.DefaultConnectionName)
                                  ?? throw new InvalidOperationException($"Connection string '{ConfigurationHelper.DefaultConnectionName}' not found in appsettings.json");
        var services = new ServiceCollection();
        
        services.Configure<PrivatBankSettings>(config.GetSection(PrivatBankSettings.SectionName));
        services.AddHttpClient<IPrivatBankService, PrivatBankService>()
            .AddStandardResilienceHandler(options =>
            {
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(45);
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromSeconds(2);
                options.Retry.BackoffType = DelayBackoffType.Exponential;
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
                options.CircuitBreaker.FailureRatio = 0.5;
                options.CircuitBreaker.MinimumThroughput = 5;
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(20);
            });
        
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));
        services.AddSingleton<ITelegramBotClient>(_ => 
            new TelegramBotClient(config["BotToken"]
            ?? throw new InvalidOperationException("BotToken not found")));

        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ISessionReader>(sp => sp.GetRequiredService<ISessionService>());
        services.AddScoped<ISessionWriter>(sp => sp.GetRequiredService<ISessionService>());
        services.AddScoped<IHistoryService, HistoryService>();
        services.AddScoped<IHistoryWriter>(sp => sp.GetRequiredService<IHistoryService>());
        services.AddScoped<IHistoryReader>(sp => sp.GetRequiredService<IHistoryService>());
        services.AddScoped<IHistoryDeleter>(sp => sp.GetRequiredService<IHistoryService>());
        services.AddSingleton<BotHandler>();
        services.AddSingleton<IBotErrorHandler, BotErrorHandler>();
        services.AddSingleton<IExceptionHandler, SqlExceptionHandler>();
        services.AddSingleton<IExceptionHandler, HttpExceptionHandler>();
        services.AddSingleton<IDateValidator, DateValidator>();
        services.AddSingleton<IHelpService, HelpService>();
        services.AddSingleton<IRateMessageFormatter, RateMessageFormatter>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ISubscriptionWriter>(sp => sp.GetRequiredService<ISubscriptionService>());
        services.AddScoped<ISubscriptionReader>(sp => sp.GetRequiredService<ISubscriptionService>());
        services.AddSingleton<HistoryCommandHandler>();
        services.AddSingleton<LastCommandHandler>();
        services.AddSingleton<NotificationService>();
        services.AddSingleton<IKeyboardFactory, KeyboardFactory>();
        services.AddSingleton<IKeyboardButton, HistoryKeyboardButton>();
        services.AddSingleton<IKeyboardButton, LastKeyboardButton>();
        services.AddSingleton<INotificationSender, NotificationSender>();
        services.AddSingleton<INotificationDataProvider, NotificationDataProvider>();
        services.AddSingleton<SubscribeCommandHandler>();
        services.AddSingleton<ICommandHandler, HelpCommandHandler>();
        services.AddSingleton<ICommandHandler, StartCommandHandler>();
        services.AddSingleton<ICommandHandler, UnsubscribeCommandHandler>();
        services.AddSingleton<ICommandHandler, DeleteDataCommandHandler>();
        services.AddSingleton<ICommandHandler, RateCommandHandler>();
        services.AddSingleton<ICommandHandler>(sp => sp.GetRequiredService<HistoryCommandHandler>());
        services.AddSingleton<ICommandHandler>(sp => sp.GetRequiredService<LastCommandHandler>());
        services.AddSingleton<ICommandHandler>(sp => sp.GetRequiredService<SubscribeCommandHandler>());
        services.AddSingleton<ICallBackHandler>(sp => sp.GetRequiredService<HistoryCommandHandler>());
        services.AddSingleton<ICallBackHandler>(sp => sp.GetRequiredService<LastCommandHandler>());
        services.AddSingleton<ICurrencyValidator, CurrencyValidator>();
        services.AddSingleton<IStateHandler, WaitingForCurrencyHandler>();
        services.AddSingleton<IStateHandler, WaitingForRateCurrencyHandler>();
        services.AddSingleton<IStateHandler, WaitingForDateHandler>();
        services.AddSingleton<IStateHandler, WaitingForSubscribeCurrencyHandler>();
        services.AddSingleton<IStateHandler, WaitingForSubscribeTimeHandler>();
        services.AddSingleton<IUserDataDeletionService, UserDataDeletionService>();
        services.AddSingleton<IRateQueryService, RateQueryService>();
        services.AddSingleton<ITodayRateService, TodayRateService>();
        services.AddSingleton<ISubscribeQueryService, SubscribeQueryService>();
        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddSingleton<IHistoryQueue, HistoryQueue>();
        services.AddSingleton<IHistoryQueueWriter>(sp => (IHistoryQueueWriter)sp.GetRequiredService<IHistoryQueue>());
        services.AddSingleton<IHistoryQueueReader>(sp => (IHistoryQueueReader)sp.GetRequiredService<IHistoryQueue>());
        services.AddSingleton<HistoryWriterService>();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        services.AddMemoryCache();
        
        var provider = services.BuildServiceProvider();
        
        var botClient = provider.GetRequiredService<ITelegramBotClient>();
        var handler = provider.GetRequiredService<BotHandler>();
        var notificationService = provider.GetRequiredService<NotificationService>();
        var historyWriter = provider.GetRequiredService<HistoryWriterService>();
        var cts = new CancellationTokenSource();
        
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            Log.Fatal(e.ExceptionObject as Exception, "Unhandled exception");
        
        try
        {
            botClient.StartReceiving(
                updateHandler: (bot, update, ct) => handler.HandleUpdateAsync(update, ct),
                errorHandler: (bot, ex, source, ct) =>
                {
                    Log.Error(ex, "Telegram error");
                    return Task.CompletedTask;
                },
                receiverOptions: new ReceiverOptions
                {
                    AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery]
                },
                cancellationToken: cts.Token
            );
            
            await botClient.SetMyDescription(
                "Бот для перегляду курсів валют ПриватБанку. " +
                "Показує актуальний та історичний курс USD, EUR, GBP до гривні.",
                languageCode: "uk");

            await botClient.SetMyShortDescription(
                "Курси валют ПриватБанку",
                languageCode: "uk");

            await botClient.SetMyCommands(
            [                                                                                                                                                                                                                                          
                new BotCommand { Command = "start", Description = "Головне меню" },
                new BotCommand { Command = "history", Description = "Історія запитів" },
                new BotCommand { Command = "rate", Description = "Поточний курс USD, EUR, GBP" },
                new BotCommand { Command = "last", Description = "Останній запит" },
                new BotCommand { Command = "help", Description = "Допомога" },
                new BotCommand { Command = "subscribe", Description = "Підписатись на курс (напр. /subscribe USD 10:00)" },
                new BotCommand { Command = "unsubscribe", Description = "Відписатись від сповіщень" },
                new BotCommand { Command = "deletedata", Description = "Видалити всі мої дані" },
            ]);
            
            await historyWriter.StartAsync(cts.Token);
            await notificationService.StartAsync(cts.Token);
            
            Log.Information("Bot started. Press Enter to stop.");
            Console.ReadLine();
            cts.Cancel();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Critical startup error");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}