using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;

namespace GymCalc.Data;

/// <summary>
/// Methods for CRUD operations on bars.
/// </summary>
public class BarRepository : GymObjectRepository
{
    /// <summary>
    /// Default selected bar weight.
    /// </summary>
    internal const decimal DEFAULT_WEIGHT = 20;

    /// <summary>
    /// Default bars weights to set up on app initialize.
    /// </summary>
    private static readonly (int, Units, bool)[] _DefaultBars =
    {
        // Metric.
        (10, Units.Kilograms, true),
        (15, Units.Kilograms, true),
        (20, Units.Kilograms, true),
        (25, Units.Kilograms, true),
        // US units.
        (15, Units.Pounds, true),
        (25, Units.Pounds, true),
        (35, Units.Pounds, true),
        (45, Units.Pounds, true),
        (55, Units.Pounds, true)
    };

    /// <summary>
    /// Object cache.
    /// </summary>
    private Dictionary<int, Bar> _cache;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Reference to the single Database object (DI).</param>
    public BarRepository(Database database) : base(database) { }

    /// <summary>
    /// Ensure the database table exist and contains some bars.
    /// </summary>
    internal override async Task Initialize()
    {
        await base.Initialize<Bar>();
    }

    internal override async Task InsertDefaults()
    {
        foreach (var (weight, units, enable) in _DefaultBars)
        {
            var bar = new Bar
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enable
            };
            await Insert(bar);
        }
    }

    /// <summary>
    /// Initialize the object cache.
    /// </summary>
    internal override async Task InitCache()
    {
        if (_cache == null)
        {
            var bars = await _Database.Connection.Table<Bar>().ToListAsync();
            var pairs = bars.Select(bar => new KeyValuePair<int, Bar>(bar.Id, bar));
            _cache = new Dictionary<int, Bar>(pairs);
        }
    }

    /// <summary>
    /// Get some bars.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<Bar>> GetSome(bool? enabled = null, bool? ascending = true,
        Units units = Units.Default)
    {
        await InitCache();
        return GetSome(_cache, enabled, ascending, units);
    }

    /// <summary>
    /// Get a bar by id.
    /// </summary>
    /// <returns></returns>
    internal override async Task<Bar> Get(int id)
    {
        await InitCache();
        return _cache[id];
    }

    /// <summary>
    /// Update a bar.
    /// </summary>
    /// <returns></returns>
    internal async Task<Bar> Update(Bar bar)
    {
        await InitCache();
        return await Update(_cache, bar);
    }

    /// <summary>
    /// Insert a new bar.
    /// </summary>
    /// <returns></returns>
    internal async Task<Bar> Insert(Bar bar)
    {
        await InitCache();
        return await Insert(_cache, bar);
    }

    /// <summary>
    /// Update or insert as required.
    /// </summary>
    /// <param name="bar"></param>
    /// <returns></returns>
    internal async Task<Bar> Upsert(Bar bar)
    {
        await InitCache();
        return await Upsert(_cache, bar);
    }

    /// <summary>
    /// Delete a bar.
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
        await base.DeleteAll<Bar>();
        _cache.Clear();
    }
}
