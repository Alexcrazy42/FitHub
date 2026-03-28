using Backend.Data;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавляем DbContext
var connectionString = builder.Configuration.GetConnectionString("Default") 
    ?? "Host=localhost;Port=5432;Database=ecommerce;Username=postgres;Password=password";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        npgsqlOptions.EnableRetryOnFailure();
    }));

// Регистрируем сервисы
builder.Services.AddScoped<FacetService>();
builder.Services.AddScoped<CategoryRepository>();

// Добавляем контроллеры
builder.Services.AddControllers();

// Добавляем NSwag для OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "FitHub Dynamic Attributes API";
    options.Version = "1.0.0";
    options.Description = "API для динамических атрибутов и фасетного поиска";
});

// CORS для фронтенда
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Применяем миграции и seed данные
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Применяем миграции
    await db.Database.MigrateAsync();
    
    // Seed данные
    await DbSeeder.SeedAsync(db);
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Health check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

app.Run();
