using FitHub.Common.Entities;

namespace FitHub.Common.AspNetCore.Accounting;

internal sealed class CurrentIdentityUserAccessor : ICurrentIdentityUserAccessor
{
    private IdentityUser? currentUser;

    private readonly ICurrentIdentityUserIdAccessor accessor;
    private readonly IIdentityUserService userService;

    public CurrentIdentityUserAccessor(ICurrentIdentityUserIdAccessor accessor, IIdentityUserService userService)
    {
        this.accessor = accessor;
        this.userService = userService;
    }

    public async Task<IdentityUser> GetCurrentIdentityUser(CancellationToken cancellationToken)
    {
        if (currentUser is not null)
        {
            return currentUser;
        }

        var userId = accessor.GetCurrentUserId();

        var user = await userService.GetOrDefaultAsync(userId, cancellationToken);

        currentUser = user ?? throw new UnexpectedException("Текущий пользователь не найден");
        return currentUser;
    }
}
