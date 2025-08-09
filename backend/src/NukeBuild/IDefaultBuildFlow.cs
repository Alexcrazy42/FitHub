using FitHub.Common.NukeBuild.BuildComponents;
using Nuke.Common;

namespace FitHub.Common.NukeBuild;

/// <summary>
/// Default build flow
/// </summary>
public interface IDefaultBuildFlow :
    IIntegrationTestsBuild,
    INuGetBuild,
    IDockerBuild,
    IReleaseBuild
{
    Target Default => _ => _
        .TryDependsOn<IReleaseBuild>(x => x.CreateRelease);
}
