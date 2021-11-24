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
public abstract class GuidEntity<T> : IEntity<Guid>, IEquatable<T>, IMutableId<Guid>
    where T : GuidEntity<T>
{
    private Guid _id;

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
    protected GuidEntity(Guid id) => _id = ValidateId(id, nameof(id));

    /// <summary>
    /// Gets or sets the value indicating whether empty GUIDs can be set on this entity type.
    /// The default value is false. You should only set this property once at program startup
    /// (in your composition root) for the corresponding entity type.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType -- I actually want this static field present for each generic type
    public static bool AllowEmptyGuid { get; set; }

    /// <summary>
    /// Gets the ID of this entity, or sets it during initialization.
    /// </summary>
    /// <exception cref="EmptyGuidException">
    /// Thrown when <paramref name="value" /> is an empty GUID and <see cref="AllowEmptyGuid" />
    /// is set to false (which is the default value).
    /// </exception>
    public Guid Id
    {
        get => _id;
        init => _id = ValidateId(value, nameof(value));
    }

    private static Guid ValidateId(Guid id, string parameterName)
    {
        if (!AllowEmptyGuid)
            id.MustNotBeEmpty(parameterName);
        return id;
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

    /// <summary>
    /// <para>
    /// Sets the Id after the entity is already initialized.
    /// </para>
    /// <para>
    /// BE CAREFUL: you must not call this method when the ID of your Entity should already be immutable.
    /// This is e.g. the case when the ID is used as a key within a dictionary.
    /// </para>
    /// <para>
    /// However, when inserting an entity into a database, the database will usually generate the ID
    /// of the entity at this point in time. Updating the ID after the insert is complete is OK and
    /// is the usual scenario where this method should be called.
    /// </para>
    /// </summary>
    /// <param name="id">The new ID for the entity.</param>
    /// <exception cref="EmptyGuidException">
    /// Thrown when <paramref name="id" /> is an empty GUID and <see cref="AllowEmptyGuid" />
    /// is set to false (which is the default value).
    /// </exception>
    void IMutableId<Guid>.SetId(Guid id) => _id = ValidateId(id, nameof(id));
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