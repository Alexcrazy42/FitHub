using GrpcServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавляем gRPC
builder.Services.AddGrpc();

// Добавляем Reflection для gRPCui
builder.Services.AddGrpcReflection();

var app = builder.Build();

// Регистрируем сервис
app.MapGrpcService<DemoServiceImpl>();

// Включаем Reflection (для Development)
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet("/", () => "gRPC Server is running. Use gRPCui to test: grpcui -plaintext localhost:5000");

app.Run();