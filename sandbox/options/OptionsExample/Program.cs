using OptionsExample.Options;

var builder = WebApplication.CreateBuilder(args);

foreach (var source in builder.Configuration.Sources)
{
    if (source is FileConfigurationSource fileSource)
    {
        Console.WriteLine($"Provider: {fileSource.GetType().Name}, Path: {fileSource.Path}");
    }
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs / endpoint mapping
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<SomeOptions>()
    .Bind(builder.Configuration.GetSection(SomeOptions.Key))
    .ValidateDataAnnotations()
    .ValidateOnStart();
    

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();