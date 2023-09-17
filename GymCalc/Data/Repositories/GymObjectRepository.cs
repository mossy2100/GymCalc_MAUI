using GymCalc.Data.Models;

namespace GymCalc.Data.Repositories;

internal abstract class GymObjectRepository
{
    /// <summary>
    /// To be implemented by concrete classes.
    /// </summary>
    internal abstract Task InsertDefaults();

    /// <summary>
    /// Ensure the database table exist and contains some objects.
    /// </summary>
    internal async Task Initialize<T>() where T : new()
    {
        var db = Database.GetConnection();

        // Create the table if it doesn't already exist.
        await db.CreateTableAsync<T>();

        // Count how many rows there are.
        var n = await db.Table<T>().CountAsync();

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
    /// Get all the gym objects of a given type.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<T>> GetAll<T>(string units, bool onlyEnabled = false,
        bool ascending = true)
        where T : GymObject, new()
    {
        var db = Database.GetConnection();

        // Get all the gym objects in the preferred units.
        var query = db.Table<T>().Where(ht => ht.Units == units);

        // Add where clause if needed.
        if (onlyEnabled)
        {
            query = query.Where(ht => ht.Enabled);
        }

        // Add order by clause.
        query = ascending
            ? query.OrderBy(ht => ht.Weight)
            : query.OrderByDescending(ht => ht.Weight);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Get a gym object of a given type and id.
    /// It's convenient to accept a 0 parameter for this.
    /// </summary>
    /// <returns></returns>
    internal async Task<T> Get<T>(int id) where T : GymObject, new()
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Cannot be less than 0.");
        }

        if (id == 0)
        {
            return null;
        }

        var db = Database.GetConnection();

        return await db.Table<T>()
            .Where(ht => ht.Id == id)
            .FirstOrDefaultAsync();
    }

    internal async Task DeleteAll<T>()
    {
        var db = Database.GetConnection();
        await db.DeleteAllAsync<T>();
    }

    internal abstract Task DeleteAll();
}
