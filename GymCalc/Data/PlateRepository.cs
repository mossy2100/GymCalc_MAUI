using GymCalc.Models;
using GymCalc.Constants;

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
    private static readonly (double, string, bool, string)[] _DefaultPlates =
    {
        // Metric.
        (0.25, Units.KILOGRAMS, false, "Red"),
        (0.5, Units.KILOGRAMS, false, "OffWhite"),
        (0.75, Units.KILOGRAMS, false, "Pink"),
        (1, Units.KILOGRAMS, false, "Green"),
        (1.25, Units.KILOGRAMS, true, "Orange"),
        (1.5, Units.KILOGRAMS, false, "Yellow"),
        (2, Units.KILOGRAMS, false, "Indigo"),
        (2.5, Units.KILOGRAMS, true, "Red"),
        (5, Units.KILOGRAMS, true, "OffWhite"),
        (7.5, Units.KILOGRAMS, false, "Pink"),
        (10, Units.KILOGRAMS, true, "Green"),
        (12.5, Units.KILOGRAMS, false, "Orange"),
        (15, Units.KILOGRAMS, true, "Yellow"),
        (20, Units.KILOGRAMS, true, "Indigo"),
        (25, Units.KILOGRAMS, true, "Red"),
        // Pounds.
        (0.25, Units.POUNDS, false, "Green"),
        (0.5, Units.POUNDS, false, "Cyan"),
        (0.75, Units.POUNDS, false, "Pink"),
        (1, Units.POUNDS, false, "OffWhite"),
        (1.25, Units.POUNDS, true, "Orange"),
        (2.5, Units.POUNDS, true, "Green"),
        (5, Units.POUNDS, true, "Cyan"),
        (10, Units.POUNDS, true, "OffWhite"),
        (15, Units.POUNDS, true, "Purple"),
        (25, Units.POUNDS, true, "Green"),
        (35, Units.POUNDS, true, "Yellow"),
        (45, Units.POUNDS, true, "Indigo"),
        (55, Units.POUNDS, true, "Red"),
    };

    public PlateRepository(Database database) : base(database)
    {
    }

    /// <summary>
    /// Ensure the database table exist and contains some bars.
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
                Units = units,
                Enabled = enabled,
                Color = color,
            };
            await conn.InsertAsync(plate);
        }
    }

    /// <summary>
    /// Get the plates.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<Plate>> GetAll(string units = Units.DEFAULT, bool? enabled = null,
        bool? ascending = null)
    {
        return await base.GetAll<Plate>(units, enabled, ascending);
    }

    /// <summary>
    /// Get a plate by id.
    /// </summary>
    /// <returns></returns>
    public async Task<Plate> Get(int id)
    {
        return await base.Get<Plate>(id);
    }

    /// <inheritdoc />
    internal override async Task DeleteAll()
    {
        await base.DeleteAll<Plate>();
    }
}
