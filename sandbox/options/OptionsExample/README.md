# Options

Несколько видов конфигов
1. Без вложенности
2. С вложенностью

Типы данных:
1. Простые: string, int, bool
2. Массивы
3. Dictionary, HashSet

Получение конфигурации:

1. IOptions, IOptionsSnapshot, IOptionsMonitor
   1.1. IOptions - не перечитывает конфигурацию, для статических настроек, которые не меняются без перезапуска
   1.2. IOptionsSnapshot - перечитывает конфигурацию при каждом запросе в Scoped/Transient, захваются Singleton классом, 
      Для настроек, которые могут обновляться в процессе жизни приложения
   1.3. IOptionsMonitor - перечитывает файл всегда в реальном времени, не захватывается singleton, динамические настройки
2. Прямое использование IConfiguration

Шпаргалка:
1. Валидация настроек на старте:

```csharp
services.AddOptions<PaymentGatewayOptions>()
    .Bind(configuration.GetSection(PaymentGatewayOptions.SectionName))
    .ValidateDataAnnotations()       // проверяет [Required], [Range] и т.д.
    .ValidateOnStart();              // проверка при старте, а не при первом обращении

-- Кастомная валидация:
services.AddOptions<PaymentGatewayOptions>()
    .Bind(configuration.GetSection(PaymentGatewayOptions.SectionName))
    .Validate(options =>
    {
        if (options.BaseUrl.StartsWith("http://"))
            return false; // продакшен только HTTPS
        return true;
    }, "PaymentGateway URL must use HTTPS in production");


-- ValidateOptions<T> (для сложной логики)
public class PaymentGatewayOptionsValidator : IValidateOptions<PaymentGatewayOptions>
{
    public ValidateOptionsResult Validate(string name, PaymentGatewayOptions options)
    {
        if (string.IsNullOrEmpty(options.BaseUrl))
            return ValidateOptionsResult.Fail("BaseUrl is required");
        
        if (options.TimeoutSeconds < 5)
            return ValidateOptionsResult.Fail("Timeout must be at least 5 seconds");
        
        return ValidateOptionsResult.Success;
    }
}

// Регистрация
services.AddSingleton<IValidateOptions<PaymentGatewayOptions>, PaymentGatewayOptionsValidator>();
```

2. Конфигурация для тестов

```csharp
// MemoryCollection (для IConfiguration)
var inMemorySettings = new Dictionary<string, string>
{
    {"PaymentGateway:BaseUrl", "https://test-gateway.local"},
    {"PaymentGateway:TimeoutSeconds", "5"},
    {"PaymentGateway:RetryCount", "1"}
};

IConfiguration configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(inMemorySettings)
    .Build();

// Прямое создание Options (без DI)
var options = Options.Create(new PaymentGatewayOptions
{
    BaseUrl = "https://test.local",
    TimeoutSeconds = 5,
    RetryCount = 1
});
// Передаёшь в тестируемый класс напрямую

// Использование WebApplicationFactory (для интеграционных тестов)
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"PaymentGateway:BaseUrl", "https://mock-gateway"},
                {"PaymentGateway:TimeoutSeconds", "2"}
            });
        });
    }
}

//  В тестах всегда стараемся передавать настройки явно, а не через файлы. Это делает тесты изолированными и быстрыми.
```


## Провайдеры конфигурации 

https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-providers

Провайдеры идут друг за другом и следующие могут переопределять предыдущих:

1. appsettings (FileConfigurationProvider, JsonConfigurationProvider как стандарт, есть еще xml, ini)

2. переменные окружения (EnvironmentVariableConfigurationProvider). Вместо двоеточия ":" используется двойное подчеркивание "__"
   Можно переопределить с помощью launchSettings при разработке
3. параметры коммандной строки (Command-line configuration provider)
4. key-per-file configuration provider
```csharp
.ConfigureAppConfiguration((_, configuration) =>
{
var path = Path.Combine(
Directory.GetCurrentDirectory(), "path/to/files");

    configuration.AddKeyPerFile(directoryPath: path, optional: true);
})
```

5. Memory configuration provider

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddInMemoryCollection(
    new Dictionary<string, string?>
    {
        ["SecretKey"] = "Dictionary MyKey Value",
        ["TransientFaultHandlingOptions:Enabled"] = bool.TrueString,
        ["TransientFaultHandlingOptions:AutoRetryDelay"] = "00:00:07",
        ["Logging:LogLevel:Default"] = "Warning"
    });

using IHost host = builder.Build();

// Application code should start here.

await host.RunAsync();
```

6. Куча кастомных провайдеров IConfiguration: AzureKeyVault, Consul, HarshiCorpVault