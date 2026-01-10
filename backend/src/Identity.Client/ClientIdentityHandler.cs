using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FitHub.Common.Identity.Client;

internal sealed class ClientIdentityHandler : ClientIdentityHandlerBase
{
    private readonly IClientIdentityOptions options;
    private readonly ILogger<ClientIdentityHandler> logger;

    public ClientIdentityHandler(IOptions<IClientIdentityOptions> options, ILogger<ClientIdentityHandler> logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is not null)
        {
            logger.LogDebug("Используется авторизация из запроса, нашу пропускаем");
            return base.SendAsync(request, cancellationToken);
        }

        if (options.AccessToken is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme.Bearer, options.AccessToken);
            logger.LogDebug("Используем токен доступа из опций");
            return base.SendAsync(request, cancellationToken);
        }

        logger.LogDebug("Не используем авторизация поскольку она не обнаружена");
        return base.SendAsync(request, cancellationToken);
    }
}
