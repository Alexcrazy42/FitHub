namespace FitHub.Common.AspNetCore.Accounting;

[Flags]
public enum IdentityUserType
{
    GymVisitor = 1,
    Trainer = 2,
    GymAdmin = 4,
    CmsAdmin = 8
}

public static class UserTypeExtensions
{
    /// <summary>
    /// Возвращает массив строковых названий ролей для заданного значения UserType.
    /// </summary>
    public static List<string> ToRoleNames(this IdentityUserType userType)
    {
        var roles = new List<string>();

        foreach (IdentityUserType role in Enum.GetValues<IdentityUserType>())
        {
            if (role == 0)
            {
                continue;
            }

            if (userType.HasFlag(role))
            {
                roles.Add(role.ToString());
            }
        }

        return roles;
    }

    /// <summary>
    /// Проверяет, содержит ли UserType указанную роль.
    /// </summary>
    public static bool HasRole(this IdentityUserType userType, IdentityUserType role)
    {
        return userType.HasFlag(role);
    }
}
