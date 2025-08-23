using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using FileStorageApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = builder.Configuration;

    return new AmazonS3Client(
        new BasicAWSCredentials(config["AWS:AccessKey"], config["AWS:SecretKey"]),
        new AmazonS3Config
        {
            ServiceURL = config["AWS:ServiceURL"],        // http://minio:9000
            ForcePathStyle = true,                        // обязательно
            UseHttp = true,                               // так как не HTTPS
            Timeout = TimeSpan.FromSeconds(20),
            MaxErrorRetry = 3,
        }
    );
});

builder.Services.AddScoped<IFileStorageService, S3FileStorageService>();
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

using (var scope = app.Services.CreateScope())
{
    var fileStorageService = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
    
    try
    {
        await fileStorageService.EnsureBucketExistsAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при создании бакета на старте приложения.");
    }
}

app.Run();