using System;
using Light.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using Synnotech.Core.DependencyInjection;

namespace Synnotech.Core.Initialization;

/// <summary>
/// Provides extension methods for registering async factories with a DI container
/// based on <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// <para>
    /// Registers an async factory for the specified type as well as the type itself.
    /// You can inject the async factory into client code to resolve
    /// an instance of your type asynchronously. When
    /// <see cref="IAsyncFactory{T}.CreateAsync" /> is called, an instance of
    /// your type is resolved and initialized asynchronously if your type implements
    /// the <see cref="IInitializeAsync" /> interface. Usually this factory is used
    /// for humble objects that perform I/O and need to be initialized asynchronously.
    /// </para>
    /// <para>
    /// <code>
    /// public class MyCaller
    /// {
    ///     public MyCaller(IAsyncFactory&lt;Session&gt; factory) =>
    ///         Factory = factory;
    ///
    ///     private IAsyncFactory&lt;Session&gt; Factory { get; }
    ///
    ///     public async Task DoSomething()
    ///     {
    ///         await using var mySession = await Factory.CreateAsync();
    ///         // Do something useful with your session here
    ///     } 
    /// }
    /// </code>
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type that needs to be instantiated via the async factory.</typeparam>
    /// <param name="services">The collection that holds all registrations for the DI container.</param>
    /// <param name="serviceLifetime">
    /// The lifetime of your instance (optional). Should be either <see cref="ServiceLifetime.Transient" /> or
    /// <see cref="ServiceLifetime.Scoped" />. The default is <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="factoryLifetime">The lifetime for the factory (optional). It's usually OK for them to be a singleton.</param>
    /// <param name="registerCreateServiceDelegate">
    /// The value indicating whether a Func&lt;T&gt; is also registered with the DI container (optional).
    /// This factory delegate is necessary for the <see cref="GenericAsyncFactory{T}" /> to work properly. The default value is true.
    /// You can set this value to false if you use a proper DI container like LightInject that offers function factories.
    /// You can also configure this value globally via <see cref="ContainerSettingsContext"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
    public static IServiceCollection AddAsyncFactoryFor<T>(this IServiceCollection services,
                                                           ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
                                                           ServiceLifetime factoryLifetime = ServiceLifetime.Singleton,
                                                           bool? registerCreateServiceDelegate = null)
        where T : class =>
        services.AddAsyncFactoryFor<T, T>(serviceLifetime, factoryLifetime, registerCreateServiceDelegate);

    /// <summary>
    /// <para>
    /// Registers an async factory for the specified type as well as the type itself.
    /// You can inject this factory into client code to resolve an instance of your type asynchronously.
    /// When <see cref="IAsyncFactory{T}.CreateAsync" /> is called, an instance of
    /// your type is resolved and initialized asynchronously if your type implements
    /// the <see cref="IInitializeAsync" /> interface. Usually this factory is used
    /// for humble objects that perform I/O and need to be initialized asynchronously.
    /// </para>
    /// <para>
    /// <code>
    /// public class MyCaller
    /// {
    ///     public MyCaller(IAsyncFactory&lt;ISession&gt; factory) =>
    ///         Factory = factory;
    ///
    ///     private IAsyncFactory&lt;ISession&gt; Factory { get; }
    ///
    ///     public async Task DoSomething()
    ///     {
    ///         await using var mySession = await Factory.CreateAsync();
    ///         // Do something useful with your session here
    ///     } 
    /// }
    /// </code>
    /// </para>
    /// </summary>
    /// <typeparam name="TAbstraction">The abstraction that your concrete type implements.</typeparam>
    /// <typeparam name="TImplementation">The type that should be instantiated by the async factory.</typeparam>
    /// <param name="services">The collection that holds all registrations for the DI container.</param>
    /// <param name="serviceLifetime">
    /// The lifetime of your instance (optional). Should be either <see cref="ServiceLifetime.Transient" /> or
    /// <see cref="ServiceLifetime.Scoped" />. The default is <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="factoryLifetime">The lifetime for the factory (optional). It's usually OK for them to be a singleton.</param>
    /// <param name="registerCreateServiceDelegate">
    /// The value indicating whether a Func&lt;TAbstraction&gt; is also registered with the DI container (optional).
    /// This factory delegate is necessary for the <see cref="GenericAsyncFactory{T}" /> to work properly. The default value is true.
    /// You can set this value to false if you use a proper DI container like LightInject that offers function factories.
    /// You can also configure this value globally via <see cref="ContainerSettingsContext"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
    public static IServiceCollection AddAsyncFactoryFor<TAbstraction, TImplementation>(this IServiceCollection services,
                                                                                       ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
                                                                                       ServiceLifetime factoryLifetime = ServiceLifetime.Singleton,
                                                                                       bool? registerCreateServiceDelegate = null)
        where TAbstraction : class
        where TImplementation : TAbstraction
    {
        services.MustNotBeNull(nameof(services));

        services.Add(new ServiceDescriptor(typeof(TAbstraction), typeof(TImplementation), serviceLifetime));
        services.Add(new ServiceDescriptor(typeof(IAsyncFactory<TAbstraction>), typeof(GenericAsyncFactory<TAbstraction>), factoryLifetime));

        if (ContainerSettingsContext.Settings.CheckIfFactoryDelegateShouldBeRegistered(registerCreateServiceDelegate))
            services.AddSingleton(container => new Func<TAbstraction>(container.GetRequiredService<TAbstraction>));

        return services;
    }
}