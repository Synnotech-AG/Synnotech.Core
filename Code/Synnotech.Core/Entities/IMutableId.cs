namespace Synnotech.Core.Entities;

/// <summary>
/// <para>
/// Represents the abstraction of an entity whose ID
/// can be set after it has been fully initialized.
/// </para>
/// <para>
/// BE CAREFUL: you must not change the ID of your Entity when it should already be immutable.
/// This is e.g. the case when the ID is used as a key within a dictionary.
/// </para>
/// </summary>
/// <typeparam name="T">The type of the ID.</typeparam>
public interface IMutableId<in T>
{
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
    void SetId(T id);
}