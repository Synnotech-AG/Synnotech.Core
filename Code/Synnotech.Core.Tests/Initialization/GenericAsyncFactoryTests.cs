using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Synnotech.Core.Initialization;
using Xunit;

namespace Synnotech.Core.Tests.Initialization;

public static class GenericAsyncFactoryTests
{
    [Fact]
    public static void MustImplementIAsyncFactory() =>
        typeof(GenericAsyncFactory<NonAsyncClass>).Should().Implement(typeof(IAsyncFactory<NonAsyncClass>));

    [Fact]
    public static async Task MustBeAbleToInstantiateNonAsyncClass()
    {
        var factory = new GenericAsyncFactory<NonAsyncClass>(CreateNonAsyncDummy);

        var instance = await factory.CreateAsync();

        instance.Should().NotBeNull();
    }

    [Fact]
    public static async Task MustBeAbleToInstantiateAsyncClass()
    {
        var factory = new GenericAsyncFactory<AsyncSpy>(CreateAsyncSpy);

        var instance = await factory.CreateAsync();
        
        instance.InitializeAsyncMustHaveBeenCalled();
    }

    [Fact]
    public static async Task MustNotReinitializeAnAlreadyInitializedInstance()
    {
        var scopedInstance = new AsyncSpy { IsInitialized = true };
        var factory = new GenericAsyncFactory<AsyncSpy>(() => scopedInstance);

        var instance = await factory.CreateAsync();

        instance.Should().BeSameAs(scopedInstance);
        instance.InitializeAsyncMustNotHaveBeenCalled();
    }

    private static NonAsyncClass CreateNonAsyncDummy() => new ();

    private static AsyncSpy CreateAsyncSpy() => new ();

    private sealed class NonAsyncClass { }

    private sealed class AsyncSpy : IInitializeAsync
    {
        private int InitializeAsyncCallCount { get; set; }
        public bool IsInitialized { get; init; }

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            InitializeAsyncCallCount++;
            return Task.CompletedTask;
        }

        public void InitializeAsyncMustHaveBeenCalled() =>
            InitializeAsyncCallCount.Should().Be(1);
        
        public void InitializeAsyncMustNotHaveBeenCalled() =>
            InitializeAsyncCallCount.Should().Be(0);
    }
}