using Microsoft.AspNetCore.Identity;
using Node;
using Quartz;
using Quartzmin;

var builder = WebApplication.CreateBuilder(args);

var nodeId = builder.Configuration["NodeId"];

// Подключаем Quartz и говорим ему брать настройки из IConfiguration (env + appsettings.json)
builder.Services.AddQuartz(q =>
{
    q.Properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
    q.Properties["quartz.scheduler.instanceName"] = $"ClusteredScheduler";
    q.Properties["quartz.scheduler.instanceId"]   = $"Node-{nodeId}"; // для Node2 → "Node2"
    q.UseDefaultThreadPool(x => x.MaxConcurrency = 5);
    q.MisfireThreshold = TimeSpan.FromSeconds(10);
    
    q.UsePersistentStore(x =>
    {
        x.UseProperties = true;
        x.UsePostgres("Server=postgres;Port=5432;Database=quartz_db;User Id=quartz_user;Password=quartz_pass;");
        x.UseSystemTextJsonSerializer();
    });
    
    // Пример: добавляем задачу и триггер
    var jobKey = new JobKey("helloJob", "demo");
    // q.AddJob<HelloJob>(j => j.WithIdentity(jobKey));
    //
    // q.AddTrigger(t => t
    //     .WithIdentity("helloTrigger", "demo")
    //     .ForJob(jobKey)
    //     .StartNow()
    //     .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever()));
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

// MVC + Swagger
builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// // Берем готовый IScheduler (он уже сконфигурирован env переменными)
// var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
// var scheduler = await schedulerFactory.GetScheduler();

app.UseRouting();

// Quartzmin UI
// app.UseQuartzmin(new QuartzminOptions
// {
//     Scheduler = scheduler,
//     VirtualPathRoot = "/quartz"
// });


app.MapGet("/", () =>
{
    var instanceId = Environment.GetEnvironmentVariable("QUARTZ__SCHEDULER__INSTANCEID") ?? "Unknown";
    return $"Quartz Node {instanceId} is running. Check /swagger for details.";
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Storage API v1");
        c.RoutePrefix = "swagger";
    });
}

app.Run();