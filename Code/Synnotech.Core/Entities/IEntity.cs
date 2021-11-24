using System;

namespace Synnotech.Core.Entities;

/// <summary>
/// Represents an entity that is identified by an ID.
/// </summary>
/// <typeparam name="T">The type of the ID.</typeparam>
public interface IEntity<out T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets the ID of the entity.
    /// </summary>
    T Id { get; }
}