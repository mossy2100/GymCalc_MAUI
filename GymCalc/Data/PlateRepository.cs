using Galaxon.Core.Enums;
using GymCalc.Models;
using GymCalc.Constants;

namespace GymCalc.Data;

/// <summary>
/// Methods for CRUD operations on plates.
/// </summary>
public class PlateRepository : GymObjectRepository
{
    /// <summary>
    /// Object cache.
    /// </summary>
    private Dictionary<int, Plate> _cache;

    /// <summary>
    /// Default plates.
    /// The key is the plate weight in kilograms. The value is the Enabled flag.
    /// Common plate weights are enabled by default. Less common ones are included but disabled.
    /// </summary>
    private static readonly (double, Units, bool, string)[] _DefaultPlates =
    {
        // Metric.
        (0.25, Units.Kilograms, false, "Red"),
        (0.5, Units.Kilograms, false, "OffWhite"),
        (0.75, Units.Kilograms, false, "Pink"),
        (1, Units.Kilograms, false, "Green"),
        (1.25, Units.Kilograms, true, "Orange"),
        (1.5, Units.Kilograms, false, "Yellow"),
        (2, Units.Kilograms, false, "Indigo"),
        (2.5, Units.Kilograms, true, "Red"),
        (5, Units.Kilograms, true, "OffWhite"),
        (7.5, Units.Kilograms, false, "Pink"),
        (10, Units.Kilograms, true, "Green"),
        (12.5, Units.Kilograms, false, "Orange"),
        (15, Units.Kilograms, true, "Yellow"),
        (20, Units.Kilograms, true, "Indigo"),
        (25, Units.Kilograms, true, "Red"),
        // Pounds.
        (0.25, Units.Pounds, false, "Green"),
        (0.5, Units.Pounds, false, "Cyan"),
        (0.75, Units.Pounds, false, "Pink"),
        (1, Units.Pounds, false, "OffWhite"),
        (1.25, Units.Pounds, true, "Orange"),
        (2.5, Units.Pounds, true, "Green"),
        (5, Units.Pounds, true, "Cyan"),
        (10, Units.Pounds, true, "OffWhite"),
        (15, Units.Pounds, true, "Purple"),
        (25, Units.Pounds, true, "Green"),
        (35, Units.Pounds, true, "Yellow"),
        (45, Units.Pounds, true, "Indigo"),
        (55, Units.Pounds, true, "Red"),
    };

    public PlateRepository(Database database) : base(database)
    {
    }

    /// <summary>
    /// Ensure the database table exist and contains some plates.
    /// </summary>
    internal override async Task Initialize()
    {
        await base.Initialize<Plate>();
    }

    internal override async Task InsertDefaults()
    {
        var conn = Database.Connection;

        foreach (var (weight, units, enabled, color) in _DefaultPlates)
        {
            var plate = new Plate
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled,
                Color = color,
            };
            await conn.InsertAsync(plate);
        }
    }

    /// <summary>
    /// Initialize the object cache.
    /// </summary>
    internal override async Task InitCache()
    {
        if (_cache == null)
        {
            var plates = await Database.Connection.Table<Plate>().ToListAsync();
            var pairs = plates.Select(plate => new KeyValuePair<int, Plate>(plate.Id, plate));
            _cache = new Dictionary<int, Plate>(pairs);
        }
    }

    /// <summary>
    /// Get some plates.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<Plate>> GetSome(Units units = Units.Default, bool? enabled = null,
        bool? ascending = null)
    {
        await InitCache();
        return GetSome(_cache, units, enabled, ascending);
    }

    /// <summary>
    /// Get a plate by id.
    /// </summary>
    /// <returns></returns>
    internal async Task<Plate> Get(int id)
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

    /// <inheritdoc />
    internal override async Task DeleteAll()
    {
        await base.DeleteAll<Plate>();
        _cache.Clear();
    }
}
