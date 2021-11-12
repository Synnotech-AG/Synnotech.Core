using System;
using FluentAssertions;
using Light.GuardClauses.Exceptions;
using Synnotech.Core.Entities;
using Xunit;

// I will need to compare sometimes variables to themselves in these tests
// ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable

namespace Synnotech.Core.Tests.Entities;

public static class GuidEntityTests
{
    [Fact]
    public static void MustImplementIEntityOfGuid() =>
        typeof(GuidEntity<>).Should().Implement<IEntity<Guid>>();

    [Fact]
    public static void TheSameInstanceShouldBeConsideredEqual()
    {
        var entity = new Entity(Guid.Parse("F46369E3-8318-4B36-BA91-641E57D99ADB"));

        (entity == entity).Should().BeTrue();
        (entity != entity).Should().BeFalse();
        entity.GetHashCode().Should().Be(entity.GetHashCode());
    }

    [Fact]
    public static void TwoInstancesWithTheSameIdShouldBeEqual()
    {
        var guid = Guid.Parse("9CBF4BEA-B01B-4C23-A576-C19CC4F0C72A");
        var x = new Entity(guid);
        var y = new Entity(guid);

        (x == y).Should().BeTrue();
        (x != y).Should().BeFalse();
        x.GetHashCode().Should().Be(y.GetHashCode());
    }

    [Fact]
    public static void TwoInstancesWithDifferentIdsShouldNotBeEqual()
    {
        var x = new Entity(Guid.Parse("A0E5F557-82E4-480A-B6E3-FD2E62B4A203"));
        var y = new Entity(Guid.Parse("B4AFC3C0-110A-41C3-84F6-4FC64C2E3470"));

        (x == y).Should().BeFalse();
        (x != y).Should().BeTrue();
        x.GetHashCode().Should().NotBe(y.GetHashCode());
    }

    [Fact]
    public static void ComparingWithNullMustReturnFalse()
    {
        var x = new Entity(Guid.Parse("CA015726-344D-4903-88B6-EBC0D21F2F20"));
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

    // I need to test some constructors without ever needing the created instance
    // ReSharper disable ObjectCreationAsStatement
#pragma warning disable CA1806 // Do not ignore method results

    [Fact]
    public static void EmptyGuidShouldThrowByDefaultViaConstructor()
    {
        Action act = () => new Entity(Guid.Empty);

        act.Should().Throw<EmptyGuidException>()
           .And.ParamName.Should().Be("id");
    }

    [Fact]
    public static void EmptyGuidShouldThrowByDefaultViaPropertyInitialization()
    {
        Action act = () => new Entity { Id = Guid.Empty };

        act.Should().Throw<EmptyGuidException>();
    }


    [Fact]
    public static void AllowEmptyGuidViaConstructor()
    {
        Entity.AllowEmptyGuid = true;

        new Entity(Guid.Empty);

        Entity.AllowEmptyGuid = false;
    }

    [Fact]
    public static void AllowEmptyGuidViaPropertyInitialization()
    {
        Entity.AllowEmptyGuid = true;

        new Entity { Id = Guid.Empty };

        Entity.AllowEmptyGuid = false;
    }

    // ReSharper restore ObjectCreationAsStatement
#pragma warning restore CA1806

    [Theory]
    [InlineData("7202ACA4-05C7-4170-A19A-8330BF184835")]
    [InlineData("946111C5-0EC5-47DD-9B77-13AED3B8C42C")]
    [InlineData("9C8277DA-5856-46FE-9299-80B8BCA9C4FC")]
    public static void ToStringShouldReturnSimpleTypeNameAndId(string guidText)
    {
        var guid = Guid.Parse(guidText);
        new Entity(guid).ToString().Should().Be("Entity " + guid);
    }

    private sealed class Entity : GuidEntity<Entity>
    {
        public Entity() { }

        public Entity(Guid id) : base(id) { }
    }
}