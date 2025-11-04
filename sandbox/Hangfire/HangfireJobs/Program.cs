using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? throw new Exception("No DB_CONNECTION_STRING environment variable");

// Настраиваем Hangfire через DI
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(connectionString, new PostgreSqlStorageOptions
    {
        PrepareSchemaIfNecessary = true
    });
});

builder.Services.AddScoped<MyBackgroundJob>();
builder.Services.AddScoped<ExportJob>();


builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 20;
    options.ShutdownTimeout = TimeSpan.FromSeconds(15);
});

var host = builder.Build();

var recurringJobManager = host.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate("my-cron-job", () => MyJobs.DoWork(), Cron.Minutely);

recurringJobManager.AddOrUpdate<MyBackgroundJob>(
    "my-recurring-job",
    job => job.RunAsync(),
    Cron.Minutely
);

var command = new ExportCommand
{
    UserId = 789,
    Format = "xlsx",
    Email = "user@example.com",
    Notify = true
};

recurringJobManager.AddOrUpdate<ExportJob>(
    "export-job",
    job => job.ProcessAsync(command),
    Cron.Minutely);

var enableDashboard = Environment.GetEnvironmentVariable("ENABLE_DASHBOARD")?.ToLower() == "true";

if (enableDashboard)
{
    var webAppBuilder = WebApplication.CreateBuilder();
    webAppBuilder.WebHost.UseUrls("http://*:5000");

    webAppBuilder.Services.AddHangfire(config =>
        config.UsePostgreSqlStorage(connectionString, new PostgreSqlStorageOptions
        {
            PrepareSchemaIfNecessary = true,
        })
    );

    var webApp = webAppBuilder.Build();

    // ✅ Добавляем фильтр
    var options = new DashboardOptions
    {
        AppPath = "/",
        DashboardTitle = "Hangfire Master Dashboard",
        Authorization = new[] { new AllowAnonymousDashboardFilter() }, // ← вот это важно,
        IsReadOnlyFunc = (context) => false  // Разрешаем ВСЕМ редактировать
    };

    webApp.UseHangfireDashboard("/hangfire", options);

    await webApp.StartAsync();
    Console.WriteLine("Dashboard доступен на http://localhost:5000/hangfire");
    await host.WaitForShutdownAsync();
}
else
{
    await host.StartAsync();
    Console.WriteLine("Worker запущен (без Dashboard)");
    await host.WaitForShutdownAsync();
}

