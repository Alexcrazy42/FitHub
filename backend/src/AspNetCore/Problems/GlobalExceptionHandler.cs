using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FitHub.AspNetCore.Problems;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService problemDetailsService;

    private readonly IReadOnlyList<IExceptionToProblemRegistry> exceptionToProblemMaps;

    private readonly ILogger logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService,
        IEnumerable<IExceptionToProblemRegistry> exceptionToProblemMaps)
    {
        this.logger = logger;
        this.problemDetailsService = problemDetailsService;
        this.exceptionToProblemMaps = exceptionToProblemMaps.ToList();
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = httpContext.Response.StatusCode,
            Title = "An error occurred",
            Type = exception.GetType().Name,
            Detail = exception.Message,
        };

        var traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;
        problemDetails.Extensions["traceId"] = traceId;

        if (!TryGetStatusCode(exception, out HttpStatusCode statusCode))
        {
            return false;
        }

        problemDetails.Status = (int)statusCode;

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return httpContext.Response.HasStarted;
    }

    private bool TryGetStatusCode(Exception exception, out HttpStatusCode statusCode)
    {
        var maps = exceptionToProblemMaps
            .Where(map => map.TryGetStatusCode(exception, out var statusCode))
            .ToList();

        if (!maps.Any())
        {
            statusCode = default;
            return false;
        }

        if (maps.Count > 1)
        {
            logger.LogWarning("Существует больше одного маппера для ошибки {ExceptionName}, применяется один, выбранный случайно", exception.GetType().Name);
        }

        maps
            .First()
            .TryGetStatusCode(exception, out statusCode);

        return true;
    }
}
