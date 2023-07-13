using GymCalc.Data.Models;

namespace GymCalc.Data.Repositories;

internal abstract class HeavyThingRepository
{
    /// <summary>
    /// Get all the heavy things of a given type.
    /// </summary>
    /// <returns></returns>
    internal static async Task<List<T>> GetAll<T>(bool onlyEnabled = false, bool ascending = true)
        where T : HeavyThing, new()
    {
        var db = Database.GetConnection();
        var query = db.Table<T>();

        // Add where clause if needed.
        if (onlyEnabled)
        {
            query = query.Where(p => p.Enabled);
        }

        // Add order by clause.
        query = ascending
            ? query.OrderBy(p => p.Weight)
            : query.OrderByDescending(p => p.Weight);

        return await query.ToListAsync();
    }
}
