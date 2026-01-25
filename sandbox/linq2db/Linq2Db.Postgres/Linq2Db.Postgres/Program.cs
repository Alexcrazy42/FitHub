using Linq2Db.Postgres.DataAccess;
using Linq2Db.Postgres.Migrations;
using Linq2Db.Postgres.Services;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Запуск миграций при старте
MigrationRunner.RunMigrations(connectionString);

// Регистрация Linq2DB
builder.Services.AddLinqToDBContext<AppDataConnection>((provider, options) =>
    options
        .UsePostgreSQL(connectionString)
        .UseDefaultLogging(provider)
);

// Регистрация сервисов
builder.Services.AddScoped<OrderService>();

// Контроллеры и Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "MyProject API", 
        Version = "v1",
        Description = "Примеры работы с Linq2DB: JOIN, подзапросы, CTE"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyProject API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();