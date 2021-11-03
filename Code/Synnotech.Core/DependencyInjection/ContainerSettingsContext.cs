using System;
using Light.GuardClauses;

namespace Synnotech.Core.DependencyInjection;

/// <summary>
/// <para>
/// Provides an ambient context for <see cref="ContainerSettings" />.
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
    /// <para>
    /// Gets or sets the current container settings.
    /// </para>
    /// <para>
    /// BE CAREFUL: this static property is not thread-safe. However, we do
    /// not recommend that your composition root is initialized across
    /// multiple threads concurrently. If you want to customize the container
    /// settings and initialize your app on multiple threads,
    /// do that on a single thread during startup and use at least
    /// a memory barrier to synchronize the setting across all CPUs.
    /// </para>
    /// </summary>
    public static ContainerSettings Settings
    {
        get => _settings ??= new ContainerSettings();
        set => _settings = value.MustNotBeNull();
    }

    /// <summary>
    /// Casts the <see cref="Settings" /> to the specified type.
    /// If the cast fails, an <see cref="InvalidCastException" /> will be thrown.
    /// </summary>
    public static T GetSettingsAs<T>()
    {
        var settings = Settings;
        if (settings is T castSettings)
            return castSettings;
        var errorMessage = $"The container settings object could not be cast to type {typeof(T)}. The settings object was:{Environment.NewLine}{settings}";
        throw new InvalidCastException(errorMessage);
    }
}