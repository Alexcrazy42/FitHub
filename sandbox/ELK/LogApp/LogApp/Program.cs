using Serilog;
using Serilog.Sinks.Network;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.TCPSink("tcp://localhost", 5000, new Serilog.Formatting.Json.JsonFormatter())
    .CreateLogger();

Log.Logger = logger;

Log.Information("Hello from .NET! Timestamp: {Timestamp}", DateTime.UtcNow);
Log.Warning("This is a warning");
Log.Error("Something went wrong");

Log.CloseAndFlush();