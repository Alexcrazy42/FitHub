using FitHub.Common.NukeBuild.BuildComponents;
using FitHub.Common.NukeBuild.Common;

namespace FitHub.Common.NukeBuild.Extensions;

public static class BuildVersioningExtensions
{
    private static ApplicationVersion? VersionCache;

    /// <summary>
    /// Use semantic versioning according to https://semver.org/
    /// </summary>
    public static ApplicationVersion UseSemanticVersion(this IBaseBuild build, int major, int minor)
    {
        if (VersionCache is not null)
        {
            return VersionCache;
        }

        var preReleaseSuffix = build.GetPreReleaseSuffix();
        VersionCache = new(
            FullVersion: $"{major}.{minor}.{build.BuildCounter}{preReleaseSuffix}",
            AssemblyVersion: $"{major}.{minor}",
            FileVersion: $"{major}.{minor}.{build.BuildCounter}",
            InformationalVersion: $"{major}.{minor}.{build.BuildCounter}{preReleaseSuffix}");

        return VersionCache;
    }
}
