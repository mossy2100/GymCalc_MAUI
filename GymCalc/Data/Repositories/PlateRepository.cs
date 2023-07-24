using GymCalc.Data.Models;
using GymCalc.Graphics;

namespace GymCalc.Data.Repositories;

/// <summary>
/// Methods for CRUD operations on plates.
/// </summary>
internal static class PlateRepository
{
    /// <summary>
    /// Default plates.
    /// The key is the plate weight in kilograms. The value is the Enabled flag.
    /// Common plate weights are enabled by default. Less common ones are included but disabled.
    /// </summary>
    private static readonly (double, string, bool, Color)[] _DefaultPlates =
    {
        // Metric.
        (0.25, Units.Kilograms, false, CustomColors.Red),
        (0.5, Units.Kilograms, false, CustomColors.OffWhite),
        (0.75, Units.Kilograms, false, CustomColors.Pink),
        (1, Units.Kilograms, false, CustomColors.Green),
        (1.25, Units.Kilograms, true, CustomColors.Orange),
        (1.5, Units.Kilograms, false, CustomColors.Yellow),
        (2, Units.Kilograms, false, CustomColors.Indigo),
        (2.5, Units.Kilograms, true, CustomColors.Red),
        (5, Units.Kilograms, true, CustomColors.OffWhite),
        (7.5, Units.Kilograms, true, CustomColors.Pink),
        (10, Units.Kilograms, true, CustomColors.Green),
        (12.5, Units.Kilograms, false, CustomColors.Orange),
        (15, Units.Kilograms, true, CustomColors.Yellow),
        (20, Units.Kilograms, true, CustomColors.Indigo),
        (25, Units.Kilograms, true, CustomColors.Red),
        // Pounds.
        (0.25, Units.Pounds, false, CustomColors.Green),
        (0.5, Units.Pounds, false, CustomColors.Blue),
        (0.75, Units.Pounds, false, CustomColors.Pink),
        (1, Units.Pounds, false, CustomColors.OffWhite),
        (1.25, Units.Pounds, true, CustomColors.Orange),
        (2.5, Units.Pounds, true, CustomColors.Green),
        (5, Units.Pounds, true, CustomColors.Blue),
        (10, Units.Pounds, true, CustomColors.OffWhite),
        (15, Units.Pounds, true, CustomColors.Purple),
        (25, Units.Pounds, true, CustomColors.Green),
        (35, Units.Pounds, true, CustomColors.Yellow),
        (45, Units.Pounds, true, CustomColors.Indigo),
        (55, Units.Pounds, true, CustomColors.Red),
    };

    /// <summary>
    /// Ensure the database table exist and contains some plates.
    /// </summary>
    internal static async Task Initialize()
    {
        var db = Database.GetConnection();

        // Create the table if it doesn't already exist.
        await db.CreateTableAsync<Plate>();

        // Count how many rows there are.
        var n = await db.Table<Plate>().CountAsync();

        // If there aren't any rows, initialize with the defaults.
        if (n == 0)
        {
            foreach (var (weight, units, enabled, color) in _DefaultPlates)
            {
                var plate = new Plate
                {
                    Weight = weight,
                    Units = units,
                    Enabled = enabled,
                    Color = color.ToHex(),
                };
                await db.InsertAsync(plate);
            }
        }
    }

    /// <summary>
    /// Get the plates.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Plate>> GetAll(string units, bool onlyEnabled = false,
        bool ascending = true)
    {
        return await HeavyThingRepository.GetAll<Plate>(units, onlyEnabled, ascending);
    }
}
