namespace ServiceA.Contracts;

public record WeatherEvent(
    string City,
    int TemperatureC,
    DateTime GeneratedAt
);