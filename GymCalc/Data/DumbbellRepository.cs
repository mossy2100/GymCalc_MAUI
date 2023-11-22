using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;

namespace GymCalc.Data;

/// <summary>
/// Methods for CRUD operations on dumbbells.
/// </summary>
public class DumbbellRepository : GymObjectRepository
{
    /// <summary>
    /// Object cache.
    /// </summary>
    private Dictionary<int, Dumbbell> _cache;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Reference to the single Database object (DI).</param>
    public DumbbellRepository(Database database) : base(database) { }

    /// <summary>
    /// Ensure the database table exist and contains some dumbbells.
    /// </summary>
    internal override async Task Initialize()
    {
        await base.Initialize<Dumbbell>();
    }

    internal override async Task InsertDefaults()
    {
        var addedSoFar = new List<(decimal, Units)>();

        // Kilograms.
        addedSoFar = await AddDumbbellSet(1, 10, 1, Units.Kilograms, true, addedSoFar);
        addedSoFar = await AddDumbbellSet(2.5m, 60, 2.5m, Units.Kilograms, true, addedSoFar);

        // Pounds.
        addedSoFar = await AddDumbbellSet(1, 10, 1, Units.Pounds, true, addedSoFar);
        addedSoFar = await AddDumbbellSet(5, 120, 5, Units.Pounds, true, addedSoFar);
    }

    private async Task<List<(decimal, Units)>> AddDumbbellSet(decimal min, decimal max,
        decimal step, Units units, bool enabled, List<(decimal, Units)> addedSoFar)
    {
        for (var weight = min; weight <= max; weight += step)
        {
            // Check we didn't add this one already.
            if (addedSoFar.Contains((weight, units)))
            {
                continue;
            }

            // Add the dumbbell.
            var dumbbell = new Dumbbell
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled,
                Color = "OffBlack"
            };
            await Insert(dumbbell);

            // Remember it.
            addedSoFar.Add((weight, units));
        }

        return addedSoFar;
    }

    /// <summary>
    /// Initialize the object cache.
    /// </summary>
    internal override async Task InitCache()
    {
        if (_cache == null)
        {
            var dumbbells = await _Database.Connection.Table<Dumbbell>().ToListAsync();
            var pairs = dumbbells.Select(dumbbell =>
                new KeyValuePair<int, Dumbbell>(dumbbell.Id, dumbbell));
            _cache = new Dictionary<int, Dumbbell>(pairs);
        }
    }

    /// <summary>
    /// Get some dumbbells.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<Dumbbell>> GetSome(bool? enabled = null, bool? ascending = true,
        Units units = Units.Default)
    {
        await InitCache();
        return GetSome(_cache, enabled, ascending, units);
    }

    /// <summary>
    /// Get a dumbbell by id.
    /// </summary>
    /// <returns></returns>
    internal override async Task<Dumbbell> Get(int id)
    {
        await InitCache();
        return _cache[id];
    }

    /// <summary>
    /// Update a dumbbell.
    /// </summary>
    /// <returns></returns>
    internal async Task<Dumbbell> Update(Dumbbell dumbbell)
    {
        await InitCache();
        return await Update(_cache, dumbbell);
    }

    /// <summary>
    /// Insert a new dumbbell.
    /// </summary>
    /// <returns></returns>
    internal async Task<Dumbbell> Insert(Dumbbell dumbbell)
    {
        await InitCache();
        return await Insert(_cache, dumbbell);
    }

    /// <summary>
    /// Update or insert as required.
    /// </summary>
    /// <param name="dumbbell"></param>
    /// <returns></returns>
    internal async Task<Dumbbell> Upsert(Dumbbell dumbbell)
    {
        await InitCache();
        return await Upsert(_cache, dumbbell);
    }

    /// <summary>
    /// Delete a dumbbell.
    /// </summary>
    /// <returns></returns>
    internal override async Task Delete(int id)
    {
        await InitCache();
        await Delete(_cache, id);
    }

    /// <inheritdoc/>
    internal override async Task DeleteAll()
    {
        await base.DeleteAll<Dumbbell>();
        _cache.Clear();
    }
}
