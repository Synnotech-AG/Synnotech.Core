namespace Synnotech.Core.DependencyInjection;

/// <summary>
/// Represents general immutable DI container settings that can be accessed
/// in static (extension) methods configuring the DI container.
/// </summary>
/// <param name="RegisterFactoryDelegates">
/// <para>
/// The value indicating whether factory delegates should be registered with the
/// DI container.
/// </para>
/// <para>A factory delegate in this context is a <c>Func&lt;T&gt;</c> that can be used
/// to call into the DI container. This way, the service container is not injected directly
/// into the corresponding component to avoid the Service Locator anti pattern.
/// By default, DI containers usually do not create registrations for delegates
/// that can be used to resolve object graphs. This is why the default value for this
/// property is true. But if you use e.g. LightInject, you can set this property to false
/// and save memory by reducing the number of registrations, because LightInject supports
/// Function Factories out of the box: https://www.lightinject.net/#function-factories 
/// </para>
/// </param>
public record ContainerSettings(bool RegisterFactoryDelegates = true)
{
    /// <summary>
    /// Checks if a factory delegate should be registered depending on the specified value.
    /// </summary>
    /// <param name="registerFactoryDelegate">
    /// An optional nullable boolean value indicating whether the caller wants to
    /// explicitly register a factory delegate.
    /// </param>
    /// <returns>
    /// If <paramref name="registerFactoryDelegate"/> is not null, its value will be returned.
    /// Otherwise, the value of <see cref="RegisterFactoryDelegates" /> is returned.
    /// </returns>
    public bool CheckIfFactoryDelegateShouldBeRegistered(bool? registerFactoryDelegate) =>
        registerFactoryDelegate ?? RegisterFactoryDelegates;
}