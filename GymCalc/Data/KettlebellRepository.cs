using GymCalc.Models;
using GymCalc.Constants;
using GymCalc.Graphics;

namespace GymCalc.Data;

/// <summary>
/// Methods for CRUD operations on kettlebells.
/// </summary>
public class KettlebellRepository : GymObjectRepository
{
    public KettlebellRepository(Database database) : base(database)
    {
    }

    /// <summary>
    /// Ensure the database table exist and contains some bars.
    /// </summary>
    internal override async Task Initialize()
    {
        await base.Initialize<Kettlebell>();
    }

    internal override async Task InsertDefaults()
    {
        var addedSoFar = new List<(double, string)>();
        // Kilograms.
        addedSoFar = await AddKettlebellSet(4, 32, 4, Units.KILOGRAMS, true, addedSoFar);
        addedSoFar = await AddKettlebellSet(6, 50, 2, Units.KILOGRAMS, false, addedSoFar);
        // Pounds.
        addedSoFar = await AddKettlebellSet(5, 60, 5, Units.POUNDS, true, addedSoFar);
        addedSoFar = await AddKettlebellSet(65, 120, 5, Units.POUNDS, false, addedSoFar);
    }

    private async Task<List<(double, string)>> AddKettlebellSet(double min, double max,
        double step, string units, bool enabled, List<(double, string)> addedSoFar)
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
                Units = units,
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
    /// Get the kettlebells.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<Kettlebell>> GetAll(string units = Units.DEFAULT,
        bool? enabled = null, bool? ascending = null)
    {
        return await base.GetAll<Kettlebell>(units, enabled, ascending);
    }

    /// <summary>
    /// Get a kettlebell by id.
    /// </summary>
    /// <returns></returns>
    public async Task<Kettlebell> Get(int id)
    {
        return await base.Get<Kettlebell>(id);
    }

    /// <inheritdoc />
    internal override async Task DeleteAll()
    {
        await base.DeleteAll<Kettlebell>();
    }
}
