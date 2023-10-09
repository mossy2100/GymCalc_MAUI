using Galaxon.Core.Enums;
using GymCalc.Constants;
using GymCalc.Models;
using GymCalc.Utilities;

namespace GymCalc.Data;

public abstract class GymObjectRepository
{
    /// <summary>
    /// Reference to the database (DI).
    /// </summary>
    protected Database Database { get; init; }

    protected GymObjectRepository(Database database)
    {
        Database = database;
    }

    /// <summary>
    /// To be implemented by concrete classes.
    /// </summary>
    internal abstract Task InsertDefaults();

    /// <summary>
    /// Ensure the database table exist and contains some objects.
    /// </summary>
    internal async Task Initialize<T>() where T : new()
    {
        var conn = Database.Connection;

        // Create the table if it doesn't already exist.
        await conn.CreateTableAsync<T>();

        // Count how many rows there are.
        var n = await conn.Table<T>().CountAsync();

        // If there aren't any rows, initialize with the defaults.
        if (n == 0)
        {
            await InsertDefaults();
        }
    }

    /// <summary>
    /// Ensure the database table exist and contains some objects.
    /// This version has the type specified in the implementation.
    /// </summary>
    internal abstract Task Initialize();

    /// <summary>
    /// Get some (or all) objects from the provided cache.
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="units"></param>
    /// <param name="enabled"></param>
    /// <param name="ascending"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    internal List<T> GetSome<T>(Dictionary<int, T> cache, Units units, bool? enabled,
        bool? ascending) where T : GymObject
    {
        // Get the bars from the cache.
        var query = cache.Select(item => item.Value);

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
            bool enabledValue = enabled.Value;
            query = query.Where(item => item.Enabled == enabledValue);
        }

        // Add order by clause if needed.
        if (ascending.HasValue)
        {
            query = ascending.Value
                ? query.OrderBy(item => item.Weight)
                : query.OrderByDescending(item => item.Weight);
        }

        return query.ToList();
    }

    /// <summary>
    /// Update a gym object.
    /// </summary>
    /// <returns>The updated gym object.</returns>
    internal async Task<T> Update<T>(Dictionary<int, T> cache, T gymObject) where T : GymObject
    {
        int nRowsUpdated = await Database.Connection.UpdateAsync(gymObject);
        if (nRowsUpdated != 1)
        {
            throw new Exception("Error updating record.");
        }
        cache[gymObject.Id] = gymObject;
        return gymObject;
    }

    /// <summary>
    /// Insert a new gym object.
    /// </summary>
    /// <returns>The new gym object.</returns>
    internal async Task<T> Insert<T>(Dictionary<int, T> cache, T gymObject) where T : GymObject
    {
        int nRowsInserted = await Database.Connection.InsertAsync(gymObject);
        if (nRowsInserted != 1)
        {
            throw new Exception("Error inserting new record.");
        }
        cache[gymObject.Id] = gymObject;
        return gymObject;
    }

    /// <summary>
    /// Update or insert as required.
    /// </summary>
    /// <returns>The new or updated gym object.</returns>
    internal async Task<T> Upsert<T>(Dictionary<int, T> cache, T gymObject) where T : GymObject
    {
        if (gymObject.Id == 0)
        {
            return await Insert(cache, gymObject);
        }
        return await Update(cache, gymObject);
    }

    /// <summary>
    /// Delete a gym object with a given type and id.
    /// </summary>
    internal async Task Delete<T>(Dictionary<int, T> cache, int id) where T : GymObject
    {
        int nRowsDeleted = await Database.Connection.DeleteAsync<T>(id);
        if (nRowsDeleted != 1)
        {
            throw new Exception("Error deleting record.");
        }
        cache.Remove(id);
    }

    internal abstract Task InitCache();

    /// <summary>
    /// Delete all objects of a given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal async Task DeleteAll<T>()
    {
        await Database.Connection.DeleteAllAsync<T>();
    }

    internal abstract Task Delete(int id);

    internal abstract Task DeleteAll();
}
