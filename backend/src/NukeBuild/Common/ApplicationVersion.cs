namespace FitHub.Common.NukeBuild.Common;

public record class ApplicationVersion(
    string FullVersion,
    string AssemblyVersion,
    string FileVersion,
    string InformationalVersion);
