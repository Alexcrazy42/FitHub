using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitHub.AspNetCore.Problems;

internal sealed class GlobalExceptionLoggingHandler : IExceptionHandler
{
    private readonly ILogger logger;
    private readonly IReadOnlyList<IExceptionToProblemRegistry> exceptionToStatusCodeMaps;

    public GlobalExceptionLoggingHandler(ILogger<GlobalExceptionLoggingHandler> logger, IEnumerable<IExceptionToProblemRegistry> exceptionToStatusCodeMaps)
    {
        this.logger = logger;
        this.exceptionToStatusCodeMaps = exceptionToStatusCodeMaps.ToList();
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        const string message = "Возникло необработанное исключение во время исполнения запроса";

        if (IsHandled(exception))
        {
            logger.LogInformation(exception, message);
        }
        else
        {
            logger.LogError(exception, message);
        }

        // Всегда возвращаем false, чтобы позволить прочим хэндлерам обработать исключение
        return ValueTask.FromResult(false);
    }

    private bool IsHandled(Exception exception)
        => exceptionToStatusCodeMaps.Any(exceptionToStatusCodeMap => exceptionToStatusCodeMap.TryGetStatusCode(exception, out _));

}
