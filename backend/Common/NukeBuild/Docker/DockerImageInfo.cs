namespace FitHub.Common.NukeBuild.Docker;

public interface IDockerImageInfo
{
    string DockerImageName { get; }
}

public sealed class DockerImageInfo(string dockerImageName, string dockerfileName) : IDockerImageInfo
{
    public string DockerImageName { get; } = dockerImageName;

    public string DockerfileName { get; } = dockerfileName;
}

public sealed class GeneratedDockerImageInfo(string dockerImageName, string projectName) : IDockerImageInfo
{
    public string DockerImageName { get; } = dockerImageName;

    public string ProjectName { get; } = projectName;

    public IReadOnlyList<string> AdditionalBuildSteps { get; init; } = [];

    public IReadOnlyList<string> AdditionalFinalSteps { get; init; } = [];
}
