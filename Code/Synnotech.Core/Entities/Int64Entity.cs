using System;

namespace Synnotech.Core.Entities;

/// <summary>
/// Represents the base class for entities that have a <see cref="long" /> ID.
/// This class implements <see cref="IEquatable{T}" /> so that two instances with
/// the same ID are considered equal. If you want to check for reference equality,
/// you must explicitly call <see cref="object.ReferenceEquals" />.
/// </summary>
/// <typeparam name="T">
/// Your type that derives from this class. This generic parameter is used for
/// the <see cref="IEquatable{T}" /> implementation.
/// </typeparam>
public abstract class Int64Entity<T> : IEntity<long>, IEquatable<T>
    where T : Int64Entity<T>
{
    /// <summary>
    /// Initializes a new instance of <see cref="Int64Entity{T}" />
    /// </summary>
    protected Int64Entity() { }

    /// <summary>
    /// Initializes a new instance of <see cref="Int64Entity{T}" /> with the specified ID.
    /// </summary>
    /// <param name="id">The ID for the entity.</param>
    protected Int64Entity(long id) => Id = id;

    /// <inheritdoc />
    public long Id { get; init; }

    /// <summary>
    /// Checks if the other instance is not null and has the same ID as this instance.
    /// </summary>
    /// <returns>True when both entities are considered equal, else false.</returns>
    public bool Equals(T? other) =>
        other is not null && Id == other.Id;

    /// <summary>
    /// Checks if the other instance is of the same entity type and has the same ID as this instance.
    /// </summary>
    /// <returns>True when both entities are considered equal, else false.</returns>
    public override bool Equals(object @object) =>
        @object is T entity && Equals(entity);

    /// <summary>
    /// Returns the Id of the entity.
    /// </summary>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Returns the simple type name and appends the ID.
    /// </summary>
    public override string ToString() => $"{GetType().Name} {Id}";

    /// <summary>
    /// Checks if two entities are equal.
    /// </summary>
    public static bool operator ==(Int64Entity<T>? left, T? right) => left?.Equals(right) ?? right is null;

    /// <summary>
    /// Checks if two entities are not equal.
    /// </summary>
    public static bool operator !=(Int64Entity<T>? left, T? right) => !(left?.Equals(right) ?? right is null);
}

/// <summary>
/// Represents a specialized base class for entities that have an <see cref="long" /> ID.
/// Consider deriving from <see cref="Int64Entity{T}" /> if you want to limit which
/// entities can be compared with each other.
/// </summary>
public abstract class Int64Entity : Int64Entity<Int64Entity>
{
    /// <summary>
    /// Initializes a new instance of <see cref="Int64Entity" />
    /// </summary>
    protected Int64Entity() { }

    /// <summary>
    /// Initializes a new instance of <see cref="Int64Entity" /> with the specified ID.
    /// </summary>
    /// <param name="id">The ID for the entity.</param>
    protected Int64Entity(long id) : base(id) { }
}