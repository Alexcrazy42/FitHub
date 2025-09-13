using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ServiceB.Data;
using ServiceB.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ServiceB API", Version = "v1" });
});

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kafka Consumer Hosted Service
builder.Services.AddHostedService<KafkaWeatherConsumer>();

// HTTP Client for calling ServiceA
builder.Services.AddHttpClient("ServiceA", client =>
{
    client.BaseAddress = new Uri("http://servicea:8080");
}); 

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(
        Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? "service-b"))
    .WithTracing(tracing =>
    {
        tracing
            .AddSource("ServiceB.KafkaConsumer")
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation();

        tracing.AddOtlpExporter();
    });


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiceB API v1");
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