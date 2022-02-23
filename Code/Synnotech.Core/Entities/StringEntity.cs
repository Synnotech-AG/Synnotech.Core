using System;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;

namespace Synnotech.Core.Entities;

/// <summary>
/// Represents the base class for entities that have a <see cref="string" /> ID.
/// This class implements <see cref="IEquatable{T}" /> so that two instances with
/// the same ID are considered equal. If you want to check for reference equality,
/// you must explicitly call <see cref="object.ReferenceEquals" />.
/// </summary>
/// <typeparam name="T">
/// Your type that derives from this class. This generic parameter is used for
/// the <see cref="IEquatable{T}" /> implementation.
/// </typeparam>
public abstract class StringEntity<T> : IEntity<string>, IEquatable<T>, IMutableId<string>
    where T : StringEntity<T>
{
    /// <summary>
    /// Represents a method that takes a string ID and a parameter name and validates
    /// the ID. If validation fails, an exception should be thrown. If everything is fine,
    /// return the value of the <paramref name="id" /> parameter.
    /// </summary>
    public delegate string ValidateIdDelegate(string id, string parameterName);

    private static ValidateIdDelegate _validateId = ValidateTrimmedNotWhiteSpaceShorterThanOrEqualTo200;
    private string? _id = IsDefaultValueNull ? null : string.Empty;

    /// <summary>
    /// Initializes a new instance of <see cref="StringEntity{T}" />
    /// </summary>
    protected StringEntity() { }

    /// <summary>
    /// Initializes a new instance of <see cref="StringEntity{T}" /> with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id" /> is invalid. By default, the
    /// id is checked for null, being empty, containing only white space,
    /// and being not trimmed. You can customize the validation process via
    /// <see cref="ValidateId" />.
    /// </exception>
    protected StringEntity(string id) => _id = ValidateId(id, nameof(id));

    // ReSharper disable StaticMemberInGenericType -- this is by design. We want to have different settings for different subtypes
    /// <summary>
    /// <para>
    /// Gets or sets the delegate that is used to validate IDs.
    /// The default value points to the <see cref="ValidateTrimmedNotWhiteSpaceShorterThanOrEqualTo200" /> method.
    /// </para>
    /// <para>
    /// You can create your own pure method that corresponds to the <see cref="ValidateIdDelegate" />
    /// to customize the string validation process. See <see cref="ValidateIdDelegate" /> for details.
    /// </para>
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value" /> is null.</exception>
    public static ValidateIdDelegate ValidateId
    {
        get => _validateId;
        set => _validateId = value.MustNotBeNull();
    }

    /// <summary>
    /// Gets or sets the value indicating whether the default value for <see cref="Id"/> is null.
    /// This defaults to true. If you set this value to false, <see cref="Id"/> will be set
    /// to <see cref="string.Empty" />.
    /// </summary>
    public static bool IsDefaultValueNull { get; set; } = true;

    /// <summary>
    /// <para>
    /// Gets or sets the mode for ID comparisons. The default is <see cref="StringComparison.Ordinal" />.
    /// </para>
    /// <para>
    /// BE CAREFUL: this property should only be set before the first entity of the corresponding
    /// subclass of <see cref="StringEntity{T}" /> is instantiated. Otherwise, this might lead to
    /// subtle bugs, e.g. when the ID is used as a key in a dictionary.
    /// </para>
    /// </summary>
    public static StringComparison ComparisonMode { get; set; } = StringComparison.Ordinal;
    // ReSharper restore StaticMemberInGenericType

    /// <summary>
    /// Gets the ID of this entity, or sets it during initialization.
    /// </summary>
    public string Id
    {
        get => _id!;
        init => _id = ValidateId(value, nameof(value));
    }

    /// <summary>
    /// Checks if the other instance is not null and has the same ID as this instance.
    /// </summary>
    /// <returns>True when both entities are considered equal, else false.</returns>
    public bool Equals(T? other) =>
        other is not null && Id.Equals(other.Id, ComparisonMode);

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
    void IMutableId<string>.SetId(string id) => _id = ValidateId(id, nameof(id));

    /// <summary>
    /// <para>
    /// Checks if the specified ID is not null, not empty, and does not contain only white space.
    /// Furthermore, the ID must be trimmed, i.e. it must not start or end with white space characters, and
    /// it must be at most 200 characters long.
    /// </para>
    /// <para>
    /// This is the default method for <see cref="ValidateId" />.
    /// </para>
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="id" /> is null.</exception>
    /// <exception cref="StringException">Thrown when the string is empty, contains only white space, or is not trimmed.</exception>
    public static string ValidateTrimmedNotWhiteSpaceShorterThanOrEqualTo200(string id, string parameterName)
    {
        id.MustNotBeNullOrWhiteSpace(parameterName);
        if (id[0].IsWhiteSpace() || id[id.Length - 1].IsWhiteSpace())
            throw new StringException(parameterName, $"The ID must be trimmed, but you specified {id}");
        if (id.Length > 200)
            throw new StringLengthException(parameterName, $"The ID should not be longer than 200 characters, but the following ID is {id.Length} characters long:{Environment.NewLine}{id}");
        return id;
    }

    /// <summary>
    /// Checks if the other instance is of the same entity type and has the same ID as this instance.
    /// </summary>
    /// <returns>True when both entities are considered equal, else false.</returns>
    public override bool Equals(object? @object) =>
        @object is T entity && Equals(entity);

    /// <summary>
    /// Returns the hashcode of the ID.
    /// </summary>
    public override int GetHashCode()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (Id is null)
            return 0;
        
        // ReSharper disable once NonReadonlyMemberInGetHashCode -- this must be handled properly by the caller.
        var comparisonMode = ComparisonMode;
        return comparisonMode switch
        {
            StringComparison.CurrentCulture => StringComparer.CurrentCulture.GetHashCode(Id),
            StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase.GetHashCode(Id),
            StringComparison.InvariantCulture => StringComparer.InvariantCulture.GetHashCode(Id),
            StringComparison.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase.GetHashCode(Id),
            StringComparison.Ordinal => Id.GetHashCode(),
            StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase.GetHashCode(Id),
            _ => throw new InvalidOperationException($"The ComparisonMode property is set to an invalid value {comparisonMode}")
        };
    }

    /// <summary>
    /// Returns the simple type name and appends the ID.
    /// </summary>
    public override string ToString() => $"{GetType().Name} {Id}";

    /// <summary>
    /// Checks if two entities are equal.
    /// </summary>
    public static bool operator ==(StringEntity<T>? left, T? right) => left?.Equals(right) ?? right is null;

    /// <summary>
    /// Checks if two entities are not equal.
    /// </summary>
    public static bool operator !=(StringEntity<T>? left, T? right) => !(left?.Equals(right) ?? right is null);
}

/// <summary>
/// Represents a specialized base class for entities that have a <see cref="string" /> ID.
/// Consider deriving from <see cref="StringEntity{T}" /> if you want to limit which
/// entities can be compared with each other.
/// </summary>
public abstract class StringEntity : StringEntity<StringEntity>
{
    /// <summary>
    /// Initializes a new instance of <see cref="StringEntity" />.
    /// </summary>
    protected StringEntity() { }

    /// <summary>
    /// Initializes a new instance of <see cref="StringEntity" /> with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    protected StringEntity(string id) : base(id) { }
}