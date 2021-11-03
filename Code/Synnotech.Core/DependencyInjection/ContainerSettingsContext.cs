using Light.GuardClauses;

namespace Synnotech.Core.DependencyInjection;

/// <summary>
/// <para>
/// Provides an ambient context for <see cref="ContainerSettings"/>.
/// These are usually used in  static (extension) methods
/// where a DI container is configured.
/// </para>
/// <para>
/// BE CAREFUL: this static class is not thread-safe. However, we do
/// not recommend that your composition root is initialized across
/// multiple threads concurrently.
/// </para>
/// </summary>
public static class ContainerSettingsContext
{
    private static ContainerSettings? _settings;

    /// <summary>
    /// Gets or sets the current container settings.
    /// </summary>
    public static ContainerSettings Settings
    {
        get => _settings ??= new ContainerSettings();
        set => _settings = value.MustNotBeNull();
    }
}