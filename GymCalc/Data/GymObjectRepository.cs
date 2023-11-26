using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;
using GymCalc.Shared;

namespace GymCalc.Data;

public abstract class GymObjectRepository<T>(Database database) : IGymObjectRepository
    where T : GymObject, new()
{
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
        for (var weight = min; weight <= max; weight += step)
        {
            // Check that we haven't added this one already.
            var gymObject = await LoadOneByWeight(weight, units);

            // If the object isn't already in the database, construct and insert it.
            if (gymObject == null)
            {
                gymObject = (T)fnCreate(weight, units, enabled);
                await Insert(gymObject);
            }
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

        // If there aren't any rows, initialize with the defaults.
        var nRows = await database.Connection.Table<T>().CountAsync();
        if (nRows == 0)
        {
            await InsertDefaults();
        }
    }

    /// <summary>
    /// Get a gym object from the database, searching by Id.
    /// </summary>
    /// <param name="id">The id of the gym object.</param>
    /// <returns>The gym object or null if not found.</returns>
    internal async Task<T?> LoadOneById(int id)
    {
        return await database.Connection.FindAsync<T>(id);
    }

    /// <summary>
    /// Get a gym object from the database, searching by weight.
    /// </summary>
    /// <param name="weight">The weight of the gym object.</param>
    /// <param name="units">The units of mass the weight is expressed in.</param>
    /// <returns>The gym object or null if not found.</returns>
    internal async Task<T?> LoadOneByWeight(decimal weight, Units units)
    {
        return await database.Connection.Table<T>().FirstOrDefaultAsync(gymObject =>
            gymObject.Weight == weight && gymObject.Units == units.GetDescription());
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
    /// <returns>The matching objects.</returns>
    internal async Task<List<T>> LoadSome(bool? enabled = true, bool? ascending = true,
        Units units = Units.Default)
    {
        // Start constructing the query to select the objects.
        var query = database.Connection.Table<T>();

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
            query = query.Where(item => item.Units == units.GetDescription());
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
        return await query.ToListAsync();
    }

    /// <summary>
    /// Load all the objects of a given gym object type T from the database, ordered by units, and
    /// optionally also by weight.
    /// </summary>
    /// <param name="ascending">If to order the results by weight.</param>
    /// <returns>The list of all objects of the type T.</returns>
    internal async Task<List<T>> LoadAll(bool? ascending = true)
    {
        return await LoadSome(null, ascending, Units.All);
    }

    /// <summary>
    /// Update a gym object.
    /// </summary>
    /// <param name="gymObject">The object to update.</param>
    /// <returns>The number of rows updated.</returns>
    internal async Task<int> Update(T gymObject)
    {
        return await database.Connection.UpdateAsync(gymObject);
    }

    /// <summary>
    /// Insert a new gym object.
    /// </summary>
    /// <param name="gymObject">The object to insert.</param>
    /// <returns>The number of rows inserted.</returns>
    internal async Task<int> Insert(T gymObject)
    {
        return await database.Connection.InsertAsync(gymObject);
    }

    /// <summary>
    /// Update or insert as required.
    /// </summary>
    /// <param name="gymObject">The object to update or insert.</param>
    /// <returns>The number of rows updated or inserted.</returns>
    internal async Task<int> Upsert(T gymObject)
    {
        return await (gymObject.Id == 0 ? Insert(gymObject) : Update(gymObject));
    }

    /// <summary>
    /// Delete a gym object with a given type and id.
    /// </summary>
    /// <param name="id">The id of the gym object to delete.</param>
    /// <returns>The number of deleted rows.</returns>
    internal async Task<int> Delete(int id)
    {
        return await database.Connection.DeleteAsync<T>(id);
    }

    /// <summary>
    /// Delete all objects of a given type.
    /// </summary>
    /// <returns>The number of deleted rows.</returns>
    public async Task<int> DeleteAll()
    {
        return await database.Connection.DeleteAllAsync<T>();
    }
}
