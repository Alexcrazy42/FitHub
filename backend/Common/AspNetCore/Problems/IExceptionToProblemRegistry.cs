using System.Net;

namespace FitHub.Common.AspNetCore.Problems;

/// <summary>
/// Регистр исключений, которые необходимо обработать как <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/>
/// </summary>
/// <remarks>
/// Наследники должны регистрироваться как Singleton
/// </remarks>
public interface IExceptionToProblemRegistry
{
    /// <summary>
    /// Попробовать получить <see cref="HttpStatusCode"/> по исключению
    /// </summary>
    public bool TryGetStatusCode(Exception exception, out HttpStatusCode statusCode);
}
