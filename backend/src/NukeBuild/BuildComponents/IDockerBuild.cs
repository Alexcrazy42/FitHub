using FitHub.Common.NukeBuild.Docker;
using FitHub.Common.NukeBuild.Extensions;
using Nuke.Common;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace FitHub.Common.NukeBuild.BuildComponents;

[ParameterPrefix(nameof(Docker))]
public interface IDockerBuild : IBaseBuild
{
    [Parameter("Docker repositories url"), Required]
    Uri RepositoriesUrl => this.GetValue(() => RepositoriesUrl);

    [Parameter("Docker repository name"), Required]
    string RepositoryName => this.GetValue(() => RepositoryName);

    [Parameter("Docker repository login")]
    string RepositoryLogin => this.GetValue(() => RepositoryLogin);

    [Parameter("Docker repository password")]
    string RepositoryPassword => this.GetValue(() => RepositoryPassword);

    const string DockerContainerArtifactsPath = "/app/artifacts";

    IReadOnlyList<IDockerImageInfo> DockerImages { get; }

    string GetDockerImageTag(string dockerImageName) => $"{RepositoriesUrl.Authority}/{RepositoryName}/{dockerImageName}:{Branch}";

    bool ShouldVerifyFormat => true;

    bool ShouldPushDockerImage => false;

    Target VerifyFormat => _ => _
        .OnlyWhenDynamic(() => ShouldVerifyFormat)
        .WhenSkipped(DependencyBehavior.Execute)
        .Executes(() =>
        {
            DotNetTasks.DotNet($"format {Solution.Path} --verify-no-changes --severity warn --no-restore");
        });

    /// <summary>
    /// Dockerfile processing pipeline: build -> create container -> copy artifacts -> remove container
    /// </summary>
    Target BuildDockerfileWithArtifacts => _ => _
        .TryDependsOn<IDockerBuild>(x => x.VerifyFormat)
        .Executes(() =>
        {
            //SetupLogging();

            foreach (var dockerImageInfo in DockerImages)
            {
                this.BuildDockerfile(dockerImageInfo);

                var containerId = this.CreateDockerContainer(dockerImageInfo.DockerImageName);
                this.CopyArtifactsFromContainer(containerId);

                this.RemoveDockerContainer(containerId);
            }
        });

    /// <summary>
    /// Push Docker image to the repository
    /// </summary>
    Target PushDockerArtifacts => _ => _
        .TryDependsOn<IIntegrationTestsBuild>(x => x.RunIntegrationTests)
        .Requires(() => RepositoriesUrl)
        .OnlyWhenDynamic(() => ShouldPushDockerImage)
        .Executes(() =>
        {
            DockerTasks.DockerLogin(settings => settings
                .SetServer(RepositoriesUrl.Authority)
                .SetUsername(RepositoryLogin)
                .SetPassword(RepositoryPassword));

            foreach (var dockerImageInfo in DockerImages)
            {
                DockerTasks.DockerPush(settings => settings.SetName(GetDockerImageTag(dockerImageInfo.DockerImageName)));
                Log.Information("Docker image {DockerImageName} was pushed to {Url}",
                    dockerImageInfo.DockerImageName, GetDockerImageTag(dockerImageInfo.DockerImageName));
            }
        });


    // TODO https://github.com/nuke-build/nuke/discussions/1455
    // DockerLogger was removed in Nuke 9.0.0 and there is no replacement for it yet
    /*
    // Workaround for logging issue in Nuke with Docker tasks
    // See more details here: https://nuke.build/faq
    static void SetupLogging() => DockerLogger = (_, s) =>
    {
        if (s.Contains("[build-error]") && !s.Contains("buildErrors.log"))
        {
            Log.Error(s);
        }
        else
        {
            Log.Debug(s);
        }
    };
    */
}
