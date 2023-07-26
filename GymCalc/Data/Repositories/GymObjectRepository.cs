using GymCalc.Data.Models;

namespace GymCalc.Data.Repositories;

internal abstract class GymObjectRepository
{
    /// <summary>
    /// Ensure the database table exist and contains some objects.
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
}