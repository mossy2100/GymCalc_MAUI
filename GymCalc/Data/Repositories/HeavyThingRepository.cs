using GymCalc.Data.Models;

namespace GymCalc.Data.Repositories;

internal abstract class HeavyThingRepository
{
    /// <summary>
    /// Get all the heavy things of a given type.
    /// </summary>
    /// <returns></returns>
    internal static async Task<List<T>> GetAll<T>(string units, bool onlyEnabled = false,
        bool ascending = true)
        where T : HeavyThing, new()
    {
        var db = Database.GetConnection();

        // Get all the heavy things with in the preferred units.
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
}
