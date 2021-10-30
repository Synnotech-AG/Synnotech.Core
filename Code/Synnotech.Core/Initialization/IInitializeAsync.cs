using System.Threading;
using System.Threading.Tasks;

namespace Synnotech.Core.Initialization;

/// <summary>
/// Represents the abstraction of an object that needs to be initialized asynchronously.
/// </summary>
public interface IInitializeAsync
{
    /// <summary>
    /// Gets the value indicating whether <see cref="InitializeAsync" /> was already called.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Runs asynchronous initialization logic.
    /// </summary>
    /// <param name="cancellationToken">The token that can cancel this asynchronous operation (optional).</param>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}