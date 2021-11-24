using System;
using Light.GuardClauses;

namespace Synnotech.Core.Entities;

/// <summary>
/// Represents the base class for entities that have an <see cref="int" /> ID.
/// This class implements <see cref="IEquatable{T}" /> so that two instances with
/// the same ID are considered equal. If you want to check for reference equality,
/// you must explicitly call <see cref="object.ReferenceEquals" />.
/// </summary>
/// <typeparam name="T">
/// Your type that derives from this class. This generic parameter is used for
/// the <see cref="IEquatable{T}" /> implementation.
/// </typeparam>
public abstract class Int32Entity<T> : IEntity<int>, IEquatable<T>, IMutableId<int>
    where T : Int32Entity<T>
{
    private int _id;

    /// <summary>
    /// Initializes a new instance of <see cref="Int32Entity{T}" />
    /// </summary>
    protected Int32Entity() { }

    /// <summary>
    /// Initializes a new instance of <see cref="Int32Entity{T}" /> with the specified ID.
    /// </summary>
    /// <param name="id">The ID for the entity.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="id" /> is zero and <see cref="AllowZero" /> is false,
    /// or when <paramref name="id" /> is negative and <see cref="AllowNegative" /> is false.
    /// </exception>
    protected Int32Entity(int id) => Id = ValidateId(id, nameof(id));

    /// <summary>
    /// Gets the ID of the entity or sets it on initialization.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="value" /> is zero and <see cref="AllowZero" /> is false,
    /// or when <paramref name="value" /> is negative and <see cref="AllowNegative" /> is false.
    /// </exception>
    public int Id
    {
        get => _id;
        init => _id = ValidateId(value, nameof(value));
    }

    /// <summary>
    /// Checks if the other instance is not null and has the same ID as this instance.
    /// </summary>
    /// <returns>True when both entities are considered equal, else false.</returns>
    public bool Equals(T? other) =>
        other is not null && Id == other.Id;

    private static int ValidateId(int id, string parameterName)
    {
        if (!AllowZero && id == 0)
            throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must not be 0.");
        if (!AllowNegative)
            id.MustNotBeLessThan(0, parameterName);
        return id;
    }

    /// <summary>
    /// Checks if the other instance is of the same entity type and has the same ID as this instance.
    /// </summary>
    /// <returns>True when both entities are considered equal, else false.</returns>
    public override bool Equals(object @object) =>
        @object is T entity && Equals(entity);

    /// <summary>
    /// Returns the Id of the entity.
    /// </summary>
    public override int GetHashCode() => Id;

    /// <summary>
    /// Returns the simple type name and appends the ID.
    /// </summary>
    public override string ToString() => $"{GetType().Name} {Id}";

    /// <summary>
    /// Checks if two entities are equal.
    /// </summary>
    public static bool operator ==(Int32Entity<T>? left, T? right) => left?.Equals(right) ?? right is null;

    /// <summary>
    /// Checks if two entities are not equal.
    /// </summary>
    public static bool operator !=(Int32Entity<T>? left, T? right) => !(left?.Equals(right) ?? right is null);

    /// <summary>
    /// Gets or sets the value indicating whether 0 is a valid ID.
    /// </summary>
    // ReSharper disable StaticMemberInGenericType -- this is by design. We want to have different settings for different subtypes
    public static bool AllowZero { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether negative values are valid IDs.
    /// </summary>
    public static bool AllowNegative { get; set; }
    // ReSharper restore StaticMemberInGenericType

    /// <summary>
    /// Sets <see cref="AllowZero" /> and <see cref="AllowNegative" /> to true.
    /// </summary>
    public static void AllowZeroAndNegativeIds() => AllowZero = AllowNegative = true;

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
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="id" /> is zero and <see cref="AllowZero" /> is false,
    /// or when <paramref name="id" /> is negative and <see cref="AllowNegative" /> is false.
    /// </exception>
    void IMutableId<int>.SetId(int id) => _id = ValidateId(id, nameof(id));
}

/// <summary>
/// Represents a specialized base class for entities that have an <see cref="int" /> ID.
/// Consider deriving from <see cref="Int32Entity{T}" /> if you want to limit which
/// entities can be compared with each other.
/// </summary>
public abstract class Int32Entity : Int32Entity<Int32Entity>
{
    /// <summary>
    /// Initializes a new instance of <see cref="Int32Entity" />
    /// </summary>
    protected Int32Entity() { }

    /// <summary>
    /// Initializes a new instance of <see cref="Int32Entity" /> with the specified ID.
    /// </summary>
    /// <param name="id">The ID for the entity.</param>
    protected Int32Entity(int id) : base(id) { }
}