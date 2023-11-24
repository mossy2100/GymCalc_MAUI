using System.Data;
using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;
using GymCalc.Shared;

namespace GymCalc.Data;

public abstract class GymObjectRepository<T>(Database database) : IGymObjectRepository where T : GymObject, new()
{
    /// <summary>
    /// The cache of database objects.
    /// </summary>
    private Dictionary<int, T> Cache { get; set; }

    /// <inheritdoc />
    public abstract Task InsertDefaults();

    /// <summary>
    /// Ensure the database table exist and contains some objects.
    /// </summary>
    internal async Task Initialize()
    {
        // Create the table if it doesn't already exist.
        await database.Connection.CreateTableAsync<T>();

        // Count how many rows there are.
        var n = await database.Connection.Table<T>().CountAsync();

        // If there aren't any rows, initialize with the defaults.
        if (n == 0)
        {
            await InsertDefaults();
        }

        // Initialize the cache.
        var gymObjects = await database.Connection.Table<T>().ToListAsync();
        var pairs =
            gymObjects.Select(gymObject => new KeyValuePair<int, T>(gymObject.Id, gymObject));
        Cache = new Dictionary<int, T>(pairs);
    }

    /// <summary>
    /// Get a gym object from the database.
    /// </summary>
    /// <param name="id">The id of the gym object.</param>
    /// <returns>The gym object.</returns>
    internal T Get(int id)
    {
        return Cache[id];
    }

    /// <summary>
    /// Get some (or all) gym objects from the database.
    /// </summary>
    /// <param name="enabled">
    /// If the method should get:
    ///     true  : enabled objects (default)
    ///     false : disabled objects
    ///     null  : both
    /// </param>
    /// <param name="ascending">
    /// If the results should be ordered:
    ///     true  : ascending by weight (default)
    ///     false : descending by weight
    ///     null  : unordered
    /// </param>
    /// <param name="units">
    /// What units the results should have (Kilograms, Pounds, All, or Default).
    /// The "Default" units are set on the Settings page (default Kilograms).
    /// </param>
    /// <returns></returns>
    internal List<T> Get(bool? enabled = true, bool? ascending = true, Units units = Units.Default)
    {
        // Get the bars from the cache.
        var query = Cache.Select(item => item.Value);

        // Add where clause for units if needed.
        if (units != Units.All)
        {
            // Get default units if necessary.
            if (units == Units.Default)
            {
                units = UnitsUtility.GetDefault();
            }
            var sUnits = units.GetDescription();
            query = query.Where(item => item.Units == sUnits);
        }

        // Add where clause for enabled/disabled weights if needed.
        if (enabled.HasValue)
        {
            var enabledValue = enabled.Value;
            query = query.Where(item => item.Enabled == enabledValue);
        }

        // Add order by clause if needed.
        if (ascending.HasValue)
        {
            query = ascending.Value
                ? query.OrderBy(item => item.Weight)
                : query.OrderByDescending(item => item.Weight);
        }

        // Convert the results to a list of the gym object type stored in the repository.
        return query.ToList();
    }

    /// <summary>
    /// Update a gym object.
    /// </summary>
    /// <returns>The updated gym object.</returns>
    internal async Task<T> Update(T gymObject)
    {
        var nRowsUpdated = await database.Connection.UpdateAsync(gymObject);
        if (nRowsUpdated != 1)
        {
            throw new DataException("Error updating record.");
        }
        Cache[gymObject.Id] = gymObject;
        return gymObject;
    }

    /// <summary>
    /// Insert a new gym object.
    /// </summary>
    /// <returns>The new gym object.</returns>
    internal async Task<T> Insert(T gymObject)
    {
        var nRowsInserted = await database.Connection.InsertAsync(gymObject);
        if (nRowsInserted != 1)
        {
            throw new DataException("Error inserting new record.");
        }
        Cache[gymObject.Id] = gymObject;
        return gymObject;
    }

    /// <summary>
    /// Update or insert as required.
    /// </summary>
    /// <returns>The new or updated gym object.</returns>
    internal async Task<T> Upsert(T gymObject)
    {
        return await (gymObject.Id == 0 ? Insert(gymObject) : Update(gymObject));
    }

    /// <summary>
    /// Delete a gym object with a given type and id.
    /// </summary>
    /// <param name="id">The id of the gym object to delete.</param>
    internal async Task Delete(int id)
    {
        var nRowsDeleted = await database.Connection.DeleteAsync<T>(id);
        if (nRowsDeleted != 1)
        {
            throw new DataException("Error deleting record.");
        }
        Cache.Remove(id);
    }

    /// <summary>
    /// Delete all objects of a given type.
    /// </summary>
    public async Task DeleteAll()
    {
        await database.Connection.DeleteAllAsync<T>();
        Cache.Clear();
    }
}
