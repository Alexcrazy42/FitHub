using Node;
using Quartz;
using Quartzmin;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuartz(q =>
{
    var connStr = builder.Configuration["Quartz:DataSource:default:ConnectionString"];
    if (string.IsNullOrWhiteSpace(connStr))
        throw new Exception("Quartz connection string is missing");
    
    q.UsePersistentStore(store =>
    {
        var connStr = builder.Configuration["Quartz:DataSource:default:ConnectionString"];
        if (string.IsNullOrWhiteSpace(connStr))
            throw new Exception("Quartz connection string is missing");
        
        store.UsePostgres(connStr);
        store.UseSystemTextJsonSerializer();
        store.UseClustering(c =>
        {
            c.CheckinInterval = TimeSpan.FromMilliseconds(7500);
            c.CheckinMisfireThreshold = TimeSpan.FromSeconds(60);
        });
    });
    
    var jobKey = new JobKey("helloJob", "demo");
    q.AddJob<HelloJob>(j => j.WithIdentity(jobKey));

    q.AddTrigger(t => t
        .WithIdentity("helloTrigger", "demo")
        .ForJob(jobKey)
        .StartNow()
        .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever()));
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();


app.UseRouting();

app.UseQuartzmin(new QuartzminOptions { Scheduler = scheduler, VirtualPathRoot = "/quartz" });

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