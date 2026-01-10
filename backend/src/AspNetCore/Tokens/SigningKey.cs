using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FitHub.Common.AspNetCore.Tokens;

public static class SigningKey
{
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
        => new(Encoding.UTF8.GetBytes(key));
}
