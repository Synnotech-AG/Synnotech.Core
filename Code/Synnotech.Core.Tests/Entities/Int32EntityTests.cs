using FluentAssertions;
using Synnotech.Core.Entities;
using Xunit;

// I will need to compare sometimes variables to themselves in these tests
// ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable

namespace Synnotech.Core.Tests.Entities;

public static class Int32EntityTests
{
    [Fact]
    public static void MustImplementIEntityOfInt32() =>
        typeof(Int32Entity<>).Should().Implement<IEntity<int>>();

    [Fact]
    public static void TheSameInstanceShouldBeConsideredEqual()
    {
        var entity = new Entity(42);

        (entity == entity).Should().BeTrue();
        (entity != entity).Should().BeFalse();
        entity.GetHashCode().Should().Be(entity.Id);
    }

    [Fact]
    public static void TwoInstancesWithTheSameIdShouldBeEqual()
    {
        var x = new Entity(80);
        var y = new Entity(80);

        (x == y).Should().BeTrue();
        (x != y).Should().BeFalse();
        x.GetHashCode().Should().Be(y.GetHashCode());
    }

    [Fact]
    public static void TwoInstancesWithDifferentIdsShouldNotBeEqual()
    {
        var x = new Entity(42);
        var y = new Entity(80);

        (x == y).Should().BeFalse();
        (x != y).Should().BeTrue();
        x.GetHashCode().Should().NotBe(y.GetHashCode());
    }

    [Fact]
    public static void ComparingWithNullMustReturnFalse()
    {
        var x = new Entity(1);
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
    [InlineData(1)]
    [InlineData(12)]
    [InlineData(5000)]
    public static void ToStringShouldReturnSimpleTypeNameAndId(int id) =>
        new Entity(id).ToString().Should().Be("Entity " + id);

    private sealed class Entity : Int32Entity<Entity>
    {
        public Entity(int id) : base(id) { }
    }
}