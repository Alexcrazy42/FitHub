using Scriban.Runtime;

namespace FitHub.Common.NukeBuild.Docker;

public sealed class DockerfileModel : ScriptObject
{
    public DockerfileModel(
        string projectToPublish,
        string assemblyName,
        bool publishArtifacts,
        IReadOnlyList<string> additionalBuildSteps,
        IReadOnlyList<string> additionalFinalSteps)
    {
        Add("ProjectToPublish", projectToPublish);
        Add("AssemblyName", assemblyName);
        Add("PublishArtifacts", publishArtifacts.ToString().ToLower());
        Add("AdditionalBuildSteps", additionalBuildSteps);
        Add("AdditionalFinalSteps", additionalFinalSteps);
    }
}
