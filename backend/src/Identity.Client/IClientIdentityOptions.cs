using FitHub.Common.Extensions.Configuration;

namespace FitHub.Common.Identity.Client;

public interface IClientIdentityOptions : IHaveConfigSection
{
    static string IHaveConfigSection.SectionName
        => throw new NotImplementedException("Переопределите в дочерних классах");

    /// <summary>
    /// Токен доступа
    /// </summary>
    public string? AccessToken { get; set; }
}
