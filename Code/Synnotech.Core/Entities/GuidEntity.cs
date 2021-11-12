using System;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;

namespace Synnotech.Core.Entities;

/// <summary>
/// Represents the base class for entities that have a <see cref="Guid" /> ID.
/// This class implements <see cref="IEquatable{T}" /> so that two instances with
/// the same ID are considered equal. If you want to check for reference equality,
/// you must explicitly call <see cref="object.ReferenceEquals" />.
/// </summary>
/// <typeparam name="T">
/// Your type that derives from this class. This generic parameter is used for
/// the <see cref="IEquatable{T}" /> implementation.
/// </typeparam>
public abstract class GuidEntity<T> : IEntity<Guid>, IEquatable<T>
    where T : GuidEntity<T>
{
    private readonly Guid _id;

    /// <summary>
    /// Initializes a new instance of <see cref="GuidEntity{T}" />.
    /// </summary>
    protected GuidEntity() { }

    /// <summary>
    /// Initializes a new instance of <see cref="GuidEntity{T}" /> with the specified ID.
    /// </summary>
    /// <param name="id">The ID for the entity.</param>
    /// <exception cref="EmptyGuidException">
    /// Thrown when <paramref name="id" /> is an empty GUID and <see cref="AllowEmptyGuid" />
    /// is set to false (which is the default value).
    /// </exception>
    protected GuidEntity(Guid id)
    {
        if (!AllowEmptyGuid)
            id.MustNotBeEmpty(nameof(id));
        _id = id;
    }

    /// <summary>
    /// Gets or sets the value indicating whether empty GUIDs can be set on this entity type.
    /// The default value is false. You should only set this property once at program startup
    /// (in your composition root) for the corresponding entity type.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType -- I actually want this static field present for each generic type
    public static bool AllowEmptyGuid { get; set; }

    /// <inheritdoc />
    public Guid Id
    {
        get => _id;
        init
        {
            if (!AllowEmptyGuid)
                value.MustNotBeEmpty();
            _id = value;
        }
    }

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
    public static bool operator ==(GuidEntity<T>? left, T? right) => left?.Equals(right) ?? right is null;

    /// <summary>
    /// Checks if two entities are not equal.
    /// </summary>
    public static bool operator !=(GuidEntity<T>? left, T? right) => !(left?.Equals(right) ?? right is null);
}

/// <summary>
/// Represents a specialized base class for entities that have an <see cref="Guid" /> ID.
/// Consider deriving from <see cref="GuidEntity{T}" /> if you want to limit which
/// entities can be compared with each other.
/// </summary>
public abstract class GuidEntity : GuidEntity<GuidEntity>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GuidEntity" />
    /// </summary>
    protected GuidEntity() { }

    /// <summary>
    /// Initializes a new instance of <see cref="GuidEntity" /> with the specified ID.
    /// </summary>
    /// <param name="id">The ID for the entity.</param>
    protected GuidEntity(Guid id) : base(id) { }
}