using GymCalc.Models;

namespace GymCalc.Data;

public abstract class GymObjectRepository
{
    /// <summary>
    /// To be implemented by concrete classes.
    /// </summary>
    internal abstract Task InsertDefaults();

    /// <summary>
    /// Reference to the database (DI).
    /// </summary>
    protected Database Database { get; set; }

    protected GymObjectRepository(Database database)
    {
        Database = database;
    }

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
    /// Get all the gym objects of a given type.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<T>> GetAll<T>(string units, bool onlyEnabled = false,
        bool ascending = true)
        where T : GymObject, new()
    {
        var conn = Database.Connection;

        // Get all the gym objects in the preferred units.
        var query = conn.Table<T>().Where(ht => ht.Units == units);

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
    /// It's convenient to allow a 0 parameter for this.
    /// </summary>
    /// <returns></returns>
    internal async Task<T> Get<T>(int id) where T : GymObject, new()
    {
        switch (id)
        {
            case < 0:
                throw new ArgumentOutOfRangeException(nameof(id), "Cannot be less than 0.");

            case 0:
                return null;

            default:
            {
                var conn = Database.Connection;

                return await conn.Table<T>()
                    .Where(ht => ht.Id == id)
                    .FirstOrDefaultAsync();
            }
        }
    }

    internal async Task DeleteAll<T>()
    {
        var conn = Database.Connection;
        await conn.DeleteAllAsync<T>();
    }

    internal abstract Task DeleteAll();
}
