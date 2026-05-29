using Confluent.Kafka;
using KafkaApi;
using Messaging.Kafka.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kafka API",
        Version = "v1"
    });
});

builder.Services.AddKafka()
    .AddProducer<string, OrderCreatedEvent>()
    .WithConfig(config =>
    {
        config.BootstrapServers = "localhost:9092";
        config.Acks = Acks.All;
        config.LingerMs = 10;
        config.AllowAutoCreateTopics = false;
    })
    .AddConsumer<OrderCreatedHandler, string, OrderCreatedEvent>("orders")
    .WithConfig(config =>
    {
        config.BootstrapServers = "localhost:9092";
        config.AllowAutoCreateTopics = false;
        config.GroupId = "orders";
        config.AutoOffsetReset = AutoOffsetReset.Earliest;
        config.SessionTimeoutMs = 45000;
        config.EnableAutoCommit = false;
    });

    // .AddProducer<string, LogEvent>()
    //     .WithConfig(config =>
    //     {
    //         config.BootstrapServers = "localhost:9092";
    //         config.Acks = Acks.Leader;
    //         config.LingerMs = 100;
    //         config.AllowAutoCreateTopics = false;
    //     })
    // .AddConsumer<LogEventHandler, string, LogEvent>("logs")
    //     .WithConfig(config =>
    //     {
    //         config.BootstrapServers = "localhost:9092";
    //         config.AutoOffsetReset = AutoOffsetReset.Earliest;
    //         config.GroupId = "logs";
    //     });


builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

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

app.Run();