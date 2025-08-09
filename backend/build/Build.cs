using System.Collections.Generic;
using FitHub.Common.NukeBuild;
using FitHub.Common.NukeBuild.Common;
using FitHub.Common.NukeBuild.Docker;
using FitHub.Common.NukeBuild.Extensions;
using Nuke.Common;

class Build : NukeBuild, IDefaultBuildFlow
{
    public string ServiceName => "FitHub";

    public ApplicationVersion Version => this.UseSemanticVersion(major: 1, minor: 1);

    public IReadOnlyList<IDockerImageInfo> DockerImages { get; } =
    [
        new GeneratedDockerImageInfo(
            dockerImageName: "fithub",
            projectName: "Host")
        {
            AdditionalBuildSteps =
            [
                // Доп шаги, которые будут происходить перед билдом
                // Для примера использования можно установить Node.js
                //"curl -fsSL https://deb.nodesource.com/setup_20.x | bash -",
                //"apt-get update && apt-get install -y nodejs && rm -rf /var/lib/apt/lists/*"
            ],
            AdditionalFinalSteps =
            [
                // Аналогично AdditionalBuildSteps - шаги, которые будут происходить
                // перед итоговой сборкой образа (например, установка зависимостей)
                // "apt-get update",
            ]
        }
        //new DockerImageInfo(DockerImageName: "docker-tests-sample", DockerfileName: "Dockerfile"),
    ];

    public List<string> TestCategoriesToIgnore => [];

    public bool ShouldPushDockerImage => true;

    public static int Main()
        => Execute<Build>(x => ((IDefaultBuildFlow)x).Default);
}
