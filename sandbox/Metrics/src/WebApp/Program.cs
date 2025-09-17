using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Metrics API",
        Version = "v1"
    });
});

builder.Host.UseMetrics(options =>
{
    options.EndpointOptions = endpointsOptions =>
    {
        endpointsOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
        endpointsOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
        endpointsOptions.EnvironmentInfoEndpointEnabled = false;
    };
});

builder.Services.AddMetricsTrackingMiddleware();
builder.Services.AddMetricsEndpoints();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Storage API v1");
        c.RoutePrefix = "swagger";
    });
}

app.MapControllers();

app.UseMetricsAllMiddleware();
app.UseMetricsAllEndpoints();

app.Run();