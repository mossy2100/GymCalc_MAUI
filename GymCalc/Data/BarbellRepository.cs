using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;

namespace GymCalc.Data;

/// <summary>
/// Methods for CRUD operations on fixed-weight barbells.
/// </summary>
public class BarbellRepository : GymObjectRepository
{
    /// <summary>
    /// Object cache.
    /// </summary>
    private Dictionary<int, Barbell> _cache;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Injected database object.</param>
    public BarbellRepository(Database database) : base(database) { }

    /// <summary>
    /// Ensure the database table exist and contains some barbells.
    /// </summary>
    internal override async Task Initialize()
    {
        await base.Initialize<Barbell>();
    }

    internal override async Task InsertDefaults()
    {
        var addedSoFar = new List<(decimal, Units)>();

        // Kilograms.
        addedSoFar = await AddBarbellSet(7.5m, 70, 2.5m, Units.Kilograms, true, addedSoFar);

        // Pounds.
        addedSoFar = await AddBarbellSet(20, 140, 5, Units.Pounds, true, addedSoFar);
    }

    private async Task<List<(decimal, Units)>> AddBarbellSet(decimal min, decimal max,
        decimal step, Units units, bool enabled, List<(decimal, Units)> addedSoFar)
    {
        for (var weight = min; weight <= max; weight += step)
        {
            // Check we didn't add this one already.
            if (addedSoFar.Contains((weight, units)))
            {
                continue;
            }

            // Add the barbell.
            var barbell = new Barbell
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled
            };
            await Insert(barbell);

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
            var barbells = await Database.Connection.Table<Barbell>().ToListAsync();
            var pairs = barbells.Select(barbell =>
                new KeyValuePair<int, Barbell>(barbell.Id, barbell));
            _cache = new Dictionary<int, Barbell>(pairs);
        }
    }

    /// <summary>
    /// Get some barbells.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<Barbell>> GetSome(bool? enabled = null, bool? ascending = true,
        Units units = Units.Default)
    {
        await InitCache();
        return GetSome(_cache, enabled, ascending, units);
    }

    /// <summary>
    /// Get a barbell by id.
    /// </summary>
    /// <returns></returns>
    internal override async Task<Barbell> Get(int id)
    {
        await InitCache();
        return _cache[id];
    }

    /// <summary>
    /// Update a barbell.
    /// </summary>
    /// <returns></returns>
    internal async Task<Barbell> Update(Barbell barbell)
    {
        await InitCache();
        return await Update(_cache, barbell);
    }

    /// <summary>
    /// Insert a new barbell.
    /// </summary>
    /// <returns></returns>
    internal async Task<Barbell> Insert(Barbell barbell)
    {
        await InitCache();
        return await Insert(_cache, barbell);
    }

    /// <summary>
    /// Update or insert as required.
    /// </summary>
    /// <param name="barbell"></param>
    /// <returns></returns>
    internal async Task<Barbell> Upsert(Barbell barbell)
    {
        await InitCache();
        return await Upsert(_cache, barbell);
    }

    /// <summary>
    /// Delete a barbell.
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
        await base.DeleteAll<Barbell>();
        _cache.Clear();
    }
}
