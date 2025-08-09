using FitHub.Common.NukeBuild.Common;
using FitHub.Common.NukeBuild.Extensions;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace FitHub.Common.NukeBuild.BuildComponents;

public interface IBaseBuild : INukeBuild
{
    [Solution]
    Solution Solution => this.GetValue(() => Solution);

    [Parameter("Build counter"), Required]
    string BuildCounter => this.GetValue(() => BuildCounter);

    [Parameter("Branch name"), Required]
    string Branch => this.GetValue(() => Branch);

    string ServiceName { get; }

    ApplicationVersion Version { get; }

    AbsolutePath ArtifactsPath => RootDirectory / ".artifacts";

    AbsolutePath BuildPath => RootDirectory / "build" / "Build";

    internal string TestFilter => String.Join('&',
        // Игнорируем интеграционные тесты всегда, тк они запускаются отдельно
        TestCategoriesToIgnore
            .Append("IntegrationTests")
            .Select(category => $"Category!={category}"));

    List<string> TestCategoriesToIgnore => [];
}
