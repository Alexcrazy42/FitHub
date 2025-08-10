using System.Net;
using FitHub.Common.Entities;

namespace FitHub.Common.AspNetCore.Problems;

internal sealed class CommonExceptionToProblemRegistry : IExceptionToProblemRegistry
{
    private static IReadOnlyDictionary<Type, HttpStatusCode> ExceptionToStatusCode => new Dictionary<Type, HttpStatusCode>
    {
        { typeof(AlreadyExistsException), HttpStatusCode.BadRequest },
        { typeof(ValidationException), HttpStatusCode.BadRequest },
        { typeof(NotFoundException), HttpStatusCode.NotFound }
    };

    public bool TryGetStatusCode(Exception exception, out HttpStatusCode statusCode)
    {
        var type = exception.GetType();
        return ExceptionToStatusCode.TryGetValue(type, out statusCode);
    }
}
