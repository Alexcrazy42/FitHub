using System;
using GrpcClient;
using GrpcWebClient.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Регистрируем gRPC клиент
builder.Services.AddGrpcClient<GrpcDemo.DemoService.DemoServiceClient>(options =>
{
    options.Address = new Uri("http://localhost:5000");
});

// Регистрируем сервис-обертку
builder.Services.AddScoped<IDemoClientService, DemoClientService>();

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();