using FitHub.Authentication;
using FitHub.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.IntegrationTests.Infrastructure;

public sealed class DataSeed
{
    public static IdentityUserId FirstCmsAdminId = IdentityUserId.Parse(Guid.NewGuid());
    public static IdentityUserId FirstGymAdminId = IdentityUserId.Parse(Guid.NewGuid());

    public IReadOnlyList<User> CmsAdmins { get; }
    public IReadOnlyList<User> GymAdmins { get; }

    public DataSeed()
    {
        CmsAdmins =
        [
            CreateUser(FirstCmsAdminId, "surname1", "name1", "email1", "password1", IdentityUserType.CmsAdmin, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow)
        ];
        GymAdmins =
        [
            CreateUser(FirstGymAdminId, "surname2", "name2", "email2", "password2", IdentityUserType.GymAdmin, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow)
        ];
    }

    public IReadOnlyList<User> AllUsers => CmsAdmins.Concat(GymAdmins).ToList();

    public async Task SeedDataAsync(TestApplication testApplication)
    {
        await using var scope = testApplication.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await dbContext.AddRangeAsync(AllUsers);
        await dbContext.SaveChangesAsync();
    }

    private static User CreateUser(
        IdentityUserId userId,
        string surname,
        string name,
        string email,
        string passwordHash,
        IdentityUserType userType,
        DateTimeOffset startRegistrationAt,
        DateTimeOffset lastSeenAt,
        DateTimeOffset createdAt)
    {
        return User.Create(
            userId,
            surname,
            name,
            email,
            passwordHash,
            userType,
            startRegistrationAt,
            lastSeenAt,
            createdAt
        );
    }
}
