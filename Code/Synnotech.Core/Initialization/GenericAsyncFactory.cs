using System;
using System.Threading;
using System.Threading.Tasks;
using Light.GuardClauses;

namespace Synnotech.Core.Initialization;

/// <summary>
/// Represents a factory that instantiates an object via a delegate
/// and checks if this object implements <see cref="IInitializeAsync" />.
/// If it does, <see cref="IInitializeAsync.InitializeAsync" /> will be called.
/// </summary>
/// <typeparam name="T">The type that should be instantiated.</typeparam>
public class GenericAsyncFactory<T> : IAsyncFactory<T>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GenericAsyncFactory{T}" />.
    /// </summary>
    /// <param name="createInstance">The delegate that creates the target object.</param>
    public GenericAsyncFactory(Func<T> createInstance) =>
        CreateInstance = createInstance.MustNotBeNull(nameof(createInstance));

    private Func<T> CreateInstance { get; }

    /// <summary>
    /// Creates the specified instance and initializes it asynchronously if it implements <see cref="IInitializeAsync" />.
    /// </summary>
    /// <param name="cancellationToken">The token that can cancel this asynchronous operation (optional).</param>
    public ValueTask<T> CreateAsync(CancellationToken cancellationToken = default)
    {
        var instance = CreateInstance();
        // ReSharper disable once HeapView.PossibleBoxingAllocation -- we do not care about boxing here
        if (instance is IInitializeAsync initializeAsync && !initializeAsync.IsInitialized)
            return InitializeInstanceAsync(initializeAsync, cancellationToken);
        return new (instance);
    }

    private static async ValueTask<T> InitializeInstanceAsync(IInitializeAsync instance, CancellationToken cancellationToken)
    {
        await instance.InitializeAsync(cancellationToken);
        return (T)instance;
    }
}