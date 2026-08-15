namespace FitHub.Common.Telemetry.Propagations;

/// <summary>
/// Singleton IDisposable, который ничего не делает при Dispose.
/// </summary>
public sealed class NoOpDisposable : IDisposable
{
    public static readonly NoOpDisposable Instance = new();

    private NoOpDisposable() { }

    public void Dispose()
    {
        // Намеренно пусто
    }
}
