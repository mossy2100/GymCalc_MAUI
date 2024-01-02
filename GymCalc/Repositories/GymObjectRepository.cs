using Galaxon.Core.Types;
using GymCalc.Enums;
using GymCalc.Models;
using GymCalc.Services;
using SQLite;

namespace GymCalc.Repositories;

public abstract class GymObjectRepository<T>(Database database) : IGymObjectRepository
    where T : GymObject, new()
{
    /// <inheritdoc/>
    public abstract Task InsertDefaults();

    /// <summary>
    /// Create a new gym object.
    /// </summary>
    /// <param name="weight">The weight value.</param>
    /// <param name="units">The weight units.</param>
    /// <param name="enabled">If the object should be enabled.</param>
    /// <returns>The new gym object.</returns>
    protected virtual T Create(decimal weight, EUnits units, bool enabled)
    {
        return new T
        {
            Weight = weight,
            Units = units,
            Enabled = enabled
        };
    }

    /// <summary>
    /// Add an object to the database.
    /// </summary>
    /// <param name="weight">The weight.</param>
    /// <param name="units">The mass units.</param>
    /// <param name="enabled">If it should be enabled by default.</param>
    protected async Task AddWeight(decimal weight, EUnits units, bool enabled)
    {
        // Check that we haven't added this one already.
        T? gymObject = await LoadByWeight(weight, units);

        // If the object isn't already in the database, construct and insert it.
        if (gymObject == null)
        {
            gymObject = Create(weight, units, enabled);
            await Insert(gymObject);
        }
    }

    /// <summary>
    /// Add a set of objects to the database.
    /// </summary>
    /// <param name="min">The minimum weight.</param>
    /// <param name="max">The maximum weight.</param>
    /// <param name="step">The difference between each weight.</param>
    /// <param name="units">The mass units.</param>
    /// <param name="enabled">If they should be enabled by default.</param>
    protected async Task AddSet(decimal min, decimal max, decimal step, EUnits units, bool enabled)
    {
        for (decimal weight = min; weight <= max; weight += step)
        {
            await AddWeight(weight, units, enabled);
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
        int nRows = await database.Connection.Table<T>().CountAsync();
        if (nRows == 0)
        {
            await InsertDefaults();
        }
    }

    /// <inheritdoc/>
    public async Task<GymObject?> LoadById(int id)
    {
        return await database.Connection.FindAsync<T>(id);
    }

    /// <summary>
    /// Get a gym object from the database, searching by weight.
    /// </summary>
    /// <param name="weight">The weight of the gym object.</param>
    /// <param name="units">The units the weight is expressed in.</param>
    /// <returns>The gym object or null if not found.</returns>
    internal async Task<T?> LoadByWeight(decimal weight, EUnits units)
    {
        // Query the database.
        return await database.Connection.Table<T>().FirstOrDefaultAsync(gymObject =>
            gymObject.Weight == weight && gymObject.Units == units);
    }

    /// <summary>
    /// Get some (or all) gym objects from the database.
    /// </summary>
    /// <param name="enabled">
    /// If the method should only get enabled objects:
    ///     true  : enabled objects (default)
    ///     false : disabled objects
    ///     null  : both
    /// </param>
    /// <param name="ascending">
    /// If the results should be ordered by weight:
    ///     true  : ascending by weight (default)
    ///     false : descending by weight
    ///     null  : unordered
    /// </param>
    /// <param name="units">
    /// What units the results should have (Kilograms, Pounds, All, or Default).
    /// The "Default" units are specified by the user on the Settings page (default Kilograms).
    /// </param>
    /// <returns>The matching objects.</returns>
    internal async Task<List<T>> LoadSome(bool? enabled = true, bool? ascending = true,
        EUnits units = EUnits.Default)
    {
        // Start constructing the query to select the objects.
        AsyncTableQuery<T>? query = database.Connection.Table<T>();

        // Add where clause for enabled/disabled weights if needed.
        if (enabled.HasValue)
        {
            // Get the enabled setting as a bool before running the query, because .Value can't be
            // converted to an SQL function.
            bool enabledValue = enabled.Value;

            // Add the where clause.
            query = query.Where(item => item.Enabled == enabledValue);
        }

        // Add where clause for units if needed.
        if (units != EUnits.All)
        {
            // Get default units if necessary.
            if (units == EUnits.Default)
            {
                units = UnitsService.GetDefaultUnits();
            }

            // Add the where clause.
            query = query.Where(item => item.Units == units);
        }
        else
        {
            // If "All" units specified, order by units before ordering by weight.
            query = query.OrderBy(item => item.Units);
        }

        // Order results ascending or descending by weight if needed.
        if (ascending.HasValue)
        {
            query = ascending.Value
                ? query.OrderBy(item => item.Weight)
                : query.OrderByDescending(item => item.Weight);
        }

        // Convert the results to a list of the gym object type stored in the repository.
        return await query.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<GymObject>> LoadAll()
    {
        List<T> gymObjects = await LoadSome(null);
        return gymObjects.Cast<GymObject>().ToList();
    }

    /// <inheritdoc/>
    public async Task<int> Update(GymObject gymObject)
    {
        return await database.Connection.UpdateAsync(gymObject);
    }

    /// <inheritdoc/>
    public async Task<int> Insert(GymObject gymObject)
    {
        return await database.Connection.InsertAsync(gymObject);
    }

    /// <inheritdoc/>
    public async Task<int> Upsert(GymObject gymObject)
    {
        return await (gymObject.Id == 0 ? Insert(gymObject) : Update(gymObject));
    }

    /// <inheritdoc/>
    public async Task<int> Delete(int id)
    {
        return await database.Connection.DeleteAsync<T>(id);
    }

    /// <inheritdoc/>
    public async Task<int> Delete(GymObject gymObject)
    {
        return await Delete(gymObject.Id);
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAll()
    {
        return await database.Connection.DeleteAllAsync<T>();
    }
}
