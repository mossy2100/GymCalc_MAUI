using System.Data;
using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;
using GymCalc.Shared;

namespace GymCalc.Data;

public abstract class GymObjectRepository<T>(Database database) : IGymObjectRepository
    where T : GymObject, new()
{
    /// <summary>
    /// The cache of gym objects. It should always match what's in the database.
    /// </summary>
    protected Dictionary<int, T>? Cache { get; set; }

    /// <inheritdoc />
    public abstract Task InsertDefaults();

    /// <summary>
    /// Add a set of objects to the database.
    /// </summary>
    /// <param name="min">The minimum weight.</param>
    /// <param name="max">The maximum weight.</param>
    /// <param name="step">The difference between each weight.</param>
    /// <param name="units">The mass units.</param>
    /// <param name="enabled">If they should be enabled by default.</param>
    /// <param name="fnCreate">Function to construct new objects.</param>
    protected async Task AddSet(decimal min, decimal max, decimal step, Units units, bool enabled,
        Func<decimal, Units, bool, GymObject> fnCreate)
    {
        var sUnits = units.GetDescription();
        for (var weight = min; weight <= max; weight += step)
        {
            // Check that we haven't added this one already.
            if (Cache!.Any(pair => pair.Value.Weight == weight && pair.Value.Units == sUnits))
            {
                continue;
            }

            // Construct and add the new gym object.
            var newObject = (T)fnCreate(weight, units, enabled);
            await Insert(newObject);
        }
    }

    /// <summary>
    /// Ensure the database table exist and contains some objects.
    /// Also initialize the in-memory object cache.
    /// </summary>
    internal async Task Initialize()
    {
        // Create the table if it doesn't already exist.
        await database.Connection.CreateTableAsync<T>();

        // Get a reference to the table.
        var table = database.Connection.Table<T>();

        // Count how many rows there are.
        var nRows = await table.CountAsync();

        // If there aren't any rows, initialize with the defaults.
        if (nRows == 0)
        {
            await InsertDefaults();
        }

        // Initialize the cache.
        Cache = new Dictionary<int, T>();
        var gymObjects = await table.ToListAsync();
        foreach (var gymObject in gymObjects)
        {
            Cache.Add(gymObject.Id, gymObject);
        }
    }

    /// <summary>
    /// Get a gym object from the database.
    /// </summary>
    /// <param name="id">The id of the gym object.</param>
    /// <returns>The gym object.</returns>
    /// <exception cref="KeyNotFoundException">
    /// If there's no dictionary entry with a key matching the provided id.
    /// </exception>
    internal T LoadOne(int id)
    {
        // Try to get the value from the cache.
        if (Cache!.TryGetValue(id, out var gymObject))
        {
            return gymObject;
        }

        throw new KeyNotFoundException(
            $"No object found in the {typeof(T).Name} cache with Id = {id}.");
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
    internal List<T> LoadSome(bool? enabled = true, bool? ascending = true,
        Units units = Units.Default)
    {
        // Get the bars from the cache.
        var query = Cache!.Select(item => item.Value);

        // Add where clause for enabled/disabled weights if needed.
        if (enabled.HasValue)
        {
            var enabledValue = enabled.Value;
            query = query.Where(item => item.Enabled == enabledValue);
        }

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
        else
        {
            // If "All" units specified, order by units before ordering by weight.
            query = query.OrderBy(item => item.Units);
        }

        // Add order by ascending or descending weight if needed.
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
    /// Load all the objects of a given gym object type T from the database, ordered by units, and
    /// optionally also by weight.
    /// </summary>
    /// <param name="ascending">If to order the results by weight.</param>
    /// <returns>The list of all objects of the type T.</returns>
    internal List<T> LoadAll(bool? ascending = true)
    {
        return LoadSome(null, ascending, Units.All);
    }

    /// <summary>
    /// Update a gym object.
    /// </summary>
    /// <returns>The updated gym object.</returns>
    internal async Task<T> Update(T gymObject)
    {
        // Update the database.
        var nRowsUpdated = await database.Connection.UpdateAsync(gymObject);
        if (nRowsUpdated != 1)
        {
            throw new DataException("Error updating record.");
        }

        // Update the cache.
        Cache![gymObject.Id] = gymObject;

        return gymObject;
    }

    /// <summary>
    /// Insert a new gym object.
    /// </summary>
    /// <returns>The new gym object.</returns>
    internal async Task<T> Insert(T gymObject)
    {
        // Update the database.
        var nRowsInserted = await database.Connection.InsertAsync(gymObject);
        if (nRowsInserted != 1)
        {
            throw new DataException("Error inserting new record.");
        }

        // Update the cache.
        Cache![gymObject.Id] = gymObject;

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
        // Update the database;
        var nRowsDeleted = await database.Connection.DeleteAsync<T>(id);
        if (nRowsDeleted != 1)
        {
            throw new DataException("Error deleting record.");
        }

        // Update the cache.
        Cache!.Remove(id);
    }

    /// <summary>
    /// Delete all objects of a given type.
    /// </summary>
    public async Task DeleteAll()
    {
        // Update the database.
        await database.Connection.DeleteAllAsync<T>();

        // Update the cache.
        Cache!.Clear();
    }
}
