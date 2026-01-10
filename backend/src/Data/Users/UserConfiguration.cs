using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Utilities.System;
using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasData(GetForSeed());
    }

    private List<User> GetForSeed()
    {
        var date = new DateTimeOffset(new DateTime(2025, 11, 3, 0, 20, 0), TimeSpan.FromHours(3));

        return [
            User.Create(IdentityUserId.Parse("a88a98f0-35e8-46c4-a38e-bf88bd5c9ebc"),
                surname: "Мамедов",
                name: "Александр",
                email: "alexcrazy42@mail.ru",
                passwordHash: "$2a$11$H9cNs1CfV.iJiv/N9hIHOe4UC/23MCB8xObp4m.wKbh7YOzmsQrjO",
                userType: IdentityUserType.CmsAdmin,
                startRegistrationAt: date,
                lastSeenAt: date,
                createdAt: date)
        ];
    }
}
