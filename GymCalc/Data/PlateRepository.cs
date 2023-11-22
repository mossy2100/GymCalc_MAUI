using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;

namespace GymCalc.Data;

/// <summary>
/// Methods for CRUD operations on plates.
/// </summary>
public class PlateRepository : GymObjectRepository
{
    /// <summary>
    /// Default plates.
    /// The key is the plate weight in kilograms. The value is the Enabled flag.
    /// Common plate weights are enabled by default. Less common ones are included but disabled.
    /// </summary>
    private static readonly (decimal, Units, bool, string)[] _DefaultPlates =
    {
        // Metric.
        (0.25m, Units.Kilograms, false, "Red"),
        (0.5m, Units.Kilograms, false, "OffWhite"),
        (0.75m, Units.Kilograms, false, "Pink"),
        (1m, Units.Kilograms, false, "Green"),
        (1.25m, Units.Kilograms, true, "Orange"),
        (1.5m, Units.Kilograms, false, "Yellow"),
        (2, Units.Kilograms, false, "Indigo"),
        (2.5m, Units.Kilograms, true, "Red"),
        (5, Units.Kilograms, true, "OffWhite"),
        (7.5m, Units.Kilograms, false, "Pink"),
        (10, Units.Kilograms, true, "Green"),
        (12.5m, Units.Kilograms, false, "Orange"),
        (15, Units.Kilograms, true, "Yellow"),
        (20, Units.Kilograms, true, "Indigo"),
        (25, Units.Kilograms, true, "Red"),
        // Pounds.
        (0.25m, Units.Pounds, false, "Green"),
        (0.5m, Units.Pounds, false, "Cyan"),
        (0.75m, Units.Pounds, false, "Pink"),
        (1, Units.Pounds, false, "OffWhite"),
        (1.25m, Units.Pounds, true, "Orange"),
        (2.5m, Units.Pounds, true, "Green"),
        (5, Units.Pounds, true, "Cyan"),
        (10, Units.Pounds, true, "OffWhite"),
        (15, Units.Pounds, true, "Purple"),
        (25, Units.Pounds, true, "Green"),
        (35, Units.Pounds, true, "Yellow"),
        (45, Units.Pounds, true, "Indigo"),
        (55, Units.Pounds, true, "Red")
    };

    /// <summary>
    /// Object cache.
    /// </summary>
    private Dictionary<int, Plate> _cache;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Reference to the single Database object (DI).</param>
    public PlateRepository(Database database) : base(database) { }

    /// <summary>
    /// Ensure the database table exist and contains some plates.
    /// </summary>
    internal override async Task Initialize()
    {
        await base.Initialize<Plate>();
    }

    internal override async Task InsertDefaults()
    {
        foreach (var (weight, units, enabled, color) in _DefaultPlates)
        {
            var plate = new Plate
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled,
                Color = color
            };
            await Insert(plate);
        }
    }

    /// <summary>
    /// Initialize the object cache.
    /// </summary>
    internal override async Task InitCache()
    {
        if (_cache == null)
        {
            var plates = await _Database.Connection.Table<Plate>().ToListAsync();
            var pairs = plates.Select(plate => new KeyValuePair<int, Plate>(plate.Id, plate));
            _cache = new Dictionary<int, Plate>(pairs);
        }
    }

    /// <summary>
    /// Get some plates.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<Plate>> GetSome(bool? enabled = null, bool? ascending = true,
        Units units = Units.Default)
    {
        await InitCache();
        return GetSome(_cache, enabled, ascending, units);
    }

    /// <summary>
    /// Get a plate by id.
    /// </summary>
    /// <returns></returns>
    internal override async Task<Plate> Get(int id)
    {
        await InitCache();
        return _cache[id];
    }

    /// <summary>
    /// Update a plate.
    /// </summary>
    /// <returns></returns>
    internal async Task<Plate> Update(Plate plate)
    {
        await InitCache();
        return await Update(_cache, plate);
    }

    /// <summary>
    /// Insert a new plate.
    /// </summary>
    /// <returns></returns>
    internal async Task<Plate> Insert(Plate plate)
    {
        await InitCache();
        return await Insert(_cache, plate);
    }

    /// <summary>
    /// Update or insert as required.
    /// </summary>
    /// <param name="plate"></param>
    /// <returns></returns>
    internal async Task<Plate> Upsert(Plate plate)
    {
        await InitCache();
        return await Upsert(_cache, plate);
    }

    /// <summary>
    /// Delete a plate.
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
        await base.DeleteAll<Plate>();
        _cache.Clear();
    }
}
