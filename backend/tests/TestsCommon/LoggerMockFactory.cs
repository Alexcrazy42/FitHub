using Microsoft.Extensions.Logging;
using Moq;

namespace FitHub.TestsCommon;

public static class LoggerMockFactory
{
    public static Mock<ILogger<T>> CreateLogger<T>()
    {
        var mockLogger = new Mock<ILogger<T>>();
        return mockLogger;
    }
}
