using GymCalc.Data.Models;

namespace GymCalc.Data.Repositories;

public class HeavyThingRepository
{
    /// <summary>
    /// Get all the heavy things of a given type.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<T>> GetAll<T>(bool onlyEnabled = false, bool ascending = true)
        where T : HeavyThing, new()
    {
        var db = Database.GetConnection();
        var query = db.Table<T>();

        if (onlyEnabled)
        {
            query = query.Where(p => p.Enabled);
        }

        if (ascending)
        {
            query = query.OrderBy(p => p.Weight);
        }
        else
        {
            query = query.OrderByDescending(p => p.Weight);
        }

        return await query.ToListAsync();
    }
}
