using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ServiceB.Data;
using ServiceB.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


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