using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ServiceA.Data;
using ServiceA.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ServiceA API", Version = "v1" });
});

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
var redisConfig = ConfigurationOptions.Parse(builder.Configuration["Cache:Redis"]);
redisConfig.AbortOnConnectFail = false;
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));
builder.Services.AddScoped(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

// Kafka Producer
builder.Services.AddSingleton<KafkaProducerService>();

// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Configuration["OTEL_SERVICE_NAME"]))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddJaegerExporter(o =>
            {
                o.AgentHost = "jaeger"; // Service name from docker-compose.yml
                o.AgentPort = 6831; 
            });
            // .AddOtlpExporter(otlp =>
            // {
            //     otlp.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
            //     otlp.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc; // ← ДОБАВЬ ЭТУ СТРОКУ
            // });
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiceA API v1");
});

app.UseRouting();
app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();