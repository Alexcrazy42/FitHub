using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Utilities.System;

namespace FitHub.Common.AspNetCore.Accounting;

internal sealed class CurrentIdentityUserIdAccessor : ICurrentIdentityUserIdAccessor
{
    private readonly IReadOnlyList<ICurrentIdentityUserIdProvider> userIdProviders;

    public CurrentIdentityUserIdAccessor(IEnumerable<ICurrentIdentityUserIdProvider> userIdProviders)
    {
        this.userIdProviders = userIdProviders.ToList();
    }

    public IdentityUserId GetCurrentUserId()
    {
        var ids = userIdProviders.Select(provider => provider.GetCurrentUserIdOrDefault())
            .Where(id => id is not null)
            .ToList();

        return ids.Count switch
        {
            1 => ids.Single().Required(),
            > 1 => throw new UnexpectedException("Обнаружено более одного идентификатора пользователя"),
            _ => throw new UnexpectedException("Идентификатор текущего пользователя не найден"),
        };
    }
}
