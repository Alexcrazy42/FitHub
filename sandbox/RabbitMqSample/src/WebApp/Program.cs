using Microsoft.OpenApi.Models;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "File Storage API",
        Version = "v1",
        Description = "API for file storage using MinIO"
    });
});

// RabbitMQ Service
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RabbitMQ API v1");
    c.RoutePrefix = "swagger";
});


app.MapControllers();
app.Run();