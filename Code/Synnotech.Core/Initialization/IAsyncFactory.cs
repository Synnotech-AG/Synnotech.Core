using System.Threading;
using System.Threading.Tasks;

namespace Synnotech.Core.Initialization;

/// <summary>
/// Represents the abstraction of a factory that creates an object
/// and initializes it asynchronously.
/// </summary>
/// <typeparam name="T">The type of the object to be created</typeparam>
public interface IAsyncFactory<T>
{
    /// <summary>
    /// Creates the specified object and initializes it asynchronously if necessary.
    /// </summary>
    /// <param name="cancellationToken">The token that can cancel this asynchronous operation (optional).</param>
    ValueTask<T> CreateAsync(CancellationToken cancellationToken = default);
}