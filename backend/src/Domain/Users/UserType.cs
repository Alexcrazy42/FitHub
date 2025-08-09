namespace FitHub.Domain.Users;

[Flags]
public enum UserType
{
    GymVisitor = 1,
    Trainer = 2,
    Admin = 4,
    CmsAdmin = 8
}

public static class UserTypeExtensions
{
    /// <summary>
    /// Возвращает массив строковых названий ролей для заданного значения UserType.
    /// </summary>
    public static List<string> ToRoleNames(this UserType userType)
    {
        var roles = new List<string>();

        foreach (UserType role in Enum.GetValues<UserType>())
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
    public static bool HasRole(this UserType userType, UserType role)
    {
        return userType.HasFlag(role);
    }
}
