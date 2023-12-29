using GymCalc.Models;

namespace GymCalc.Repositories;

internal interface IGymObjectRepository
{
    /// <summary>
    /// Insert the default objects into the database table corresponding to the repository.
    /// </summary>
    public Task InsertDefaults();

    /// <summary>
    /// Load all objects from the database table corresponding to the repository.
    /// </summary>
    /// <returns>A list of all the objects in the table as GymObjects.</returns>
    public Task<List<GymObject>> LoadAll();

    /// <summary>
    /// Update a gym object.
    /// </summary>
    /// <param name="gymObject">The object to update.</param>
    /// <returns>The number of records updated (should be 0 or 1).</returns>
    public Task<int> Update(GymObject gymObject);

    /// <summary>
    /// Insert a new gym object. The object will be updated with the new id.
    /// </summary>
    /// <param name="gymObject">The object to insert.</param>
    /// <returns>The number of records inserted (should be 1).</returns>
    public Task<int> Insert(GymObject gymObject);

    /// <summary>
    /// Update or insert as required.
    /// </summary>
    /// <param name="gymObject">The object to update or insert.</param>
    /// <returns>The number of records updated or inserted (should be 0 or 1).</returns>
    public Task<int> Upsert(GymObject gymObject);

    /// <summary>
    /// Delete a gym object with a given type and id.
    /// </summary>
    /// <param name="id">The id of the gym object to delete.</param>
    /// <returns>The number of deleted records (should be 0 or 1).</returns>
    public Task<int> Delete(int id);

    /// <summary>
    /// Delete a gym object with a given type and id.
    /// </summary>
    /// <param name="gymObject">The object to delete.</param>
    /// <returns>The number of deleted records (should be 0 or 1).</returns>
    public Task<int> Delete(GymObject gymObject);

    /// <summary>
    /// Delete all objects from the database table corresponding to the repository.
    /// </summary>
    public Task<int> DeleteAll();
}
