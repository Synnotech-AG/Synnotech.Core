using FluentAssertions;
using Synnotech.Core.Entities;
using Xunit;

// I will need to compare sometimes variables to themselves in these tests
// ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable

namespace Synnotech.Core.Tests.Entities;

public static class Int64EntityTests
{
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
    [InlineData(-359_544L)]
    [InlineData(10001L)]
    public static void ToStringShouldReturnSimpleTypeNameAndId(long id) =>
        new Entity(id).ToString().Should().Be("Entity " + id);

    private sealed class Entity : Int64Entity<Entity>
    {
        public Entity(long id) : base(id) { }
    }
}