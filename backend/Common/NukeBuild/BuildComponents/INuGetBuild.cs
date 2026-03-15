using FitHub.Common.NukeBuild.Extensions;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace FitHub.Common.NukeBuild.BuildComponents;

[ParameterPrefix(nameof(NuGet))]
public interface INuGetBuild : IBaseBuild
{
    [Parameter("NuGet feed url"), Required] Uri Url => this.GetValue(() => Url);

    [Parameter("NuGet API key"), Required]
    //[Secret]
    string ApiKey => this.GetValue(() => ApiKey);

    AbsolutePath NuGetArtifactsPath => ArtifactsPath / "nuget";

    Target PushNuGetArtifacts => _ => _
        .Requires(() => Url)
        .Requires(() => ApiKey)
        .TryDependsOn<IIntegrationTestsBuild>(x => x.RunIntegrationTests)
        // Проверяем, что папка существует и в ней хотя бы один .nupkg
        .OnlyWhenDynamic(() => Directory.Exists(NuGetArtifactsPath)
                               && Directory.EnumerateFiles(NuGetArtifactsPath, "*.nupkg").Any())
        .Executes(() =>
        {
            var artifactsToPush = NuGetArtifactsPath
                .GetFiles("*.nupkg")
                .ToList();

            DotNetTasks.DotNetNuGetPush(settings =>
                settings
                    .SetTargetPath(NuGetArtifactsPath / "*.nupkg")
                    .SetSource(Url.AbsoluteUri)
                    .SetApiKey(ApiKey)
                    .EnableSkipDuplicate()
                    .EnableForceEnglishOutput());

            Log.Information("Nuget artifacts were successfully pushed: {Artifacts}", artifactsToPush.Select(x => x.Name));
        });

}
