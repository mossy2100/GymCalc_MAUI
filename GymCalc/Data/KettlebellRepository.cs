using Galaxon.Core.Enums;
using GymCalc.Models;
using GymCalc.Constants;
using GymCalc.Graphics;

namespace GymCalc.Data;

/// <summary>
/// Methods for CRUD operations on kettlebells.
/// </summary>
public class KettlebellRepository : GymObjectRepository
{
    /// <summary>
    /// Object cache.
    /// </summary>
    private Dictionary<int, Kettlebell> _cache;

    public KettlebellRepository(Database database) : base(database)
    {
    }

    /// <summary>
    /// Ensure the database table exist and contains some kettlebells.
    /// </summary>
    internal override async Task Initialize()
    {
        await base.Initialize<Kettlebell>();
    }

    internal override async Task InsertDefaults()
    {
        var addedSoFar = new List<(double, Units)>();
        // Kilograms.
        addedSoFar = await AddKettlebellSet(4, 32, 4, Units.Kilograms, true, addedSoFar);
        addedSoFar = await AddKettlebellSet(6, 50, 2, Units.Kilograms, false, addedSoFar);
        // Pounds.
        addedSoFar = await AddKettlebellSet(5, 60, 5, Units.Pounds, true, addedSoFar);
        addedSoFar = await AddKettlebellSet(65, 120, 5, Units.Pounds, false, addedSoFar);
    }

    private async Task<List<(double, Units)>> AddKettlebellSet(double min, double max,
        double step, Units units, bool enabled, List<(double, Units)> addedSoFar)
    {
        var conn = Database.Connection;

        for (var weight = min; weight <= max; weight += step)
        {
            // Check we didn't add this one already.
            if (addedSoFar.Contains((weight, units)))
            {
                continue;
            }

            // Get the default color parameters.
            var (ballColor, hasBands, bandColor) =
                CustomColors.DefaultKettlebellColor(weight, units);

            // Add the kettlebell.
            var kettlebell = new Kettlebell
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled,
                BallColor = ballColor,
                HasBands = hasBands,
                BandColor = bandColor,
            };
            await conn.InsertAsync(kettlebell);

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
            var kettlebells = await Database.Connection.Table<Kettlebell>().ToListAsync();
            var pairs = kettlebells.Select(kettlebell =>
                new KeyValuePair<int, Kettlebell>(kettlebell.Id, kettlebell));
            _cache = new Dictionary<int, Kettlebell>(pairs);
        }
    }

    /// <summary>
    /// Get some kettlebells.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<Kettlebell>> GetSome(Units units = Units.Default, bool? enabled = null,
        bool? ascending = null)
    {
        await InitCache();
        return GetSome(_cache, units, enabled, ascending);
    }

    /// <summary>
    /// Get a kettlebell by id.
    /// </summary>
    /// <returns></returns>
    internal override async Task<Kettlebell> Get(int id)
    {
        await InitCache();
        return _cache[id];
    }

    /// <summary>
    /// Update a kettlebell.
    /// </summary>
    /// <returns></returns>
    internal async Task<Kettlebell> Update(Kettlebell kettlebell)
    {
        await InitCache();
        return await Update(_cache, kettlebell);
    }

    /// <summary>
    /// Insert a new kettlebell.
    /// </summary>
    /// <returns></returns>
    internal async Task<Kettlebell> Insert(Kettlebell kettlebell)
    {
        await InitCache();
        return await Insert(_cache, kettlebell);
    }

    /// <summary>
    /// Update or insert as required.
    /// </summary>
    /// <param name="kettlebell"></param>
    /// <returns></returns>
    internal async Task<Kettlebell> Upsert(Kettlebell kettlebell)
    {
        await InitCache();
        return await Upsert(_cache, kettlebell);
    }

    /// <summary>
    /// Delete a kettlebell.
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
        await base.DeleteAll<Kettlebell>();
        _cache.Clear();
    }
}
