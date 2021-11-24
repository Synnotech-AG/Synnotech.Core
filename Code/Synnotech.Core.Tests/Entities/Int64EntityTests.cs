using System;
using FluentAssertions;
using Synnotech.Core.Entities;
using Synnotech.Xunit;
using Xunit;
using Xunit.Abstractions;

// I will need to compare sometimes variables to themselves in these tests
// ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable

namespace Synnotech.Core.Tests.Entities;

public sealed class Int64EntityTests
{
    public Int64EntityTests(ITestOutputHelper output) => Output = output;

    private ITestOutputHelper Output { get; }

    [Fact]
    public static void MustImplementIEntityOfInt64() =>
        typeof(Int64Entity<>).Should().Implement<IEntity<long>>();

    [Fact]
    public static void TheSameInstanceShouldBeConsideredEqual()
    {
        var entity = new Entity(42L);

        (entity == entity).Should().BeTrue();
        (entity != entity).Should().BeFalse();
        entity.GetHashCode().Should().Be(entity.GetHashCode());
    }

    [Fact]
    public static void TwoInstancesWithTheSameIdShouldBeEqual()
    {
        var x = new Entity(50_203_040_004L);
        var y = new Entity(50_203_040_004L);

        (x == y).Should().BeTrue();
        (x != y).Should().BeFalse();
        x.GetHashCode().Should().Be(y.GetHashCode());
    }

    [Fact]
    public static void TwoInstancesWithDifferentIdsShouldNotBeEqual()
    {
        var x = new Entity(1L);
        var y = new Entity(2L);

        (x == y).Should().BeFalse();
        (x != y).Should().BeTrue();
        x.GetHashCode().Should().NotBe(y.GetHashCode());
    }

    [Fact]
    public static void ComparingWithNullMustReturnFalse()
    {
        var x = new Entity(3L);
        var y = default(Entity);

        (x == y).Should().BeFalse();
        (x != y).Should().BeTrue();
        (y == x).Should().BeFalse();
        (y != x).Should().BeTrue();
    }

    [Fact]
    public static void TwoNullReferencesAreEqual()
    {
        var @null = default(Entity);

        (@null == @null).Should().BeTrue();
        (@null != @null).Should().BeFalse();
    }

    [Theory]
    [InlineData(5L)]
    [InlineData(359_544L)]
    [InlineData(10001L)]
    public static void ToStringShouldReturnSimpleTypeNameAndId(long id) =>
        new Entity(id).ToString().Should().Be("Entity " + id);

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public void ConstructorShouldThrowWhenPassingNegativeId(long negativeId)
    {
        void Act() => _ = new Entity(negativeId);

        ExpectArgumentOutOfRangeException(Act);
    }

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public void PropertyInitializerShouldThrowWhenPassingNegativeId(long negativeId)
    {
        void Act() => _ = new Entity { Id = negativeId };

        ExpectArgumentOutOfRangeException(Act, "value");
    }

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public static void AllowNegativeIdViaConstructor(long negativeId)
    {
        try
        {
            Entity.AllowNegativeIds = true;
            var entity = new Entity(negativeId);
            entity.Id.Should().Be(negativeId);
        }
        finally
        {
            Entity.AllowNegativeIds = false;
        }
    }

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public static void AllowNegativeIdViaPropertyInitializer(long negativeId)
    {
        try
        {
            Entity.AllowNegativeIds = true;
            var entity = new Entity { Id = negativeId };
            entity.Id.Should().Be(negativeId);
        }
        finally
        {
            Entity.AllowNegativeIds = false;
        }
    }

    [Fact]
    public void ConstructorShouldThrowWhenZeroIsPassed()
    {
        void Act() => _ = new Entity(0L);

        ExpectArgumentOutOfRangeException(Act);
    }

    [Fact]
    public void PropertyInitializerShouldThrowWhenZeroIsPassed()
    {
        void Act() => _ = new Entity { Id = 0L };

        ExpectArgumentOutOfRangeException(Act, "value");
    }

    [Fact]
    public static void AllowZeroIdViaConstructor()
    {
        try
        {
            Entity.AllowIdZero = true;
            var entity = new Entity(0L);
            entity.Id.Should().Be(0L);
        }
        finally
        {
            Entity.AllowIdZero = false;
        }
    }

    [Fact]
    public static void AllowZeroViaPropertyInitialization()
    {
        try
        {
            Entity.AllowIdZero = true;
            var entity = new Entity { Id = 0L };
            entity.Id.Should().Be(0L);
        }
        finally
        {
            Entity.AllowIdZero = false;
        }
    }

    [Fact]
    public static void SetAllowZeroAndAllowNegativeInOneCall()
    {
        try
        {
            Entity.AllowZeroAndNegativeIds();
            Entity.AllowIdZero.Should().BeTrue();
            Entity.AllowNegativeIds.Should().BeTrue();
        }
        finally
        {
            Entity.AllowNegativeIds = false;
            Entity.AllowIdZero = false;
        }
    }

    [Fact]
    public static void SetIdAfterInitialization()
    {
        var entity = new Entity();

        entity.ToMutable().SetId(42L);

        entity.Id.Should().Be(42L);
    }

    [Fact]
    public void SetIdAfterInitializationShouldThrowWhenIdIsZero()
    {
        void Act() => new Entity().ToMutable().SetId(0L);

        ExpectArgumentOutOfRangeException(Act);
    }

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public void SetIdAfterInitializationShouldThrowWhenIdIsNegative(long negativeId)
    {
        void Act() => new Entity().ToMutable().SetId(negativeId);

        ExpectArgumentOutOfRangeException(Act);
    }

    [Fact]
    public static void AllowZeroOnSetIdAfterInitialization()
    {
        try
        {
            Entity.AllowIdZero = true;
            var entity = new Entity();
            entity.ToMutable().SetId(0L);
            entity.Id.Should().Be(0L);
        }
        finally
        {
            Entity.AllowIdZero = false;
        }
    }

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public static void AllowNegativeIdsOnSetIdAfterInitialization(long negativeId)
    {
        try
        {
            Entity.AllowNegativeIds = true;
            var entity = new Entity();
            entity.ToMutable().SetId(negativeId);
            entity.Id.Should().Be(negativeId);
        }
        finally
        {
            Entity.AllowNegativeIds = false;
        }
    }

    private void ExpectArgumentOutOfRangeException(Action act, string parameterName = "id")
    {
        var exception = act.Should().Throw<ArgumentOutOfRangeException>().Which;
        exception.ShouldBeWrittenTo(Output);
        exception.ParamName.Should().Be(parameterName);
    }

    public static TheoryData<long> NegativeIds { get; } =
        new()
        {
            -1L,
            -58192923421L,
            long.MinValue
        };

    private sealed class Entity : Int64Entity<Entity>
    {
        public Entity() { }
        public Entity(long id) : base(id) { }
    }
}