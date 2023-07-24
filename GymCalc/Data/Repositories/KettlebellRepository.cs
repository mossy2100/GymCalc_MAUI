using GymCalc.Data.Models;
using GymCalc.Graphics;

namespace GymCalc.Data.Repositories;

/// <summary>
/// Methods for CRUD operations on kettlebells.
/// </summary>
internal static class KettlebellRepository
{
    /// <summary>
    /// Ensure the database table exist and contains some kettlebells.
    /// </summary>
    internal static async Task Initialize()
    {
        var db = Database.GetConnection();

        // Create the table if it doesn't already exist.
        await db.CreateTableAsync<Kettlebell>();

        // Count how many rows there are.
        var n = await db.Table<Kettlebell>().CountAsync();

        // If there aren't any rows, initialize with the defaults.
        if (n == 0)
        {
            var addedSoFar = new List<(double, string)>();
            // Kilograms.
            addedSoFar = await AddKettlebellSet(4, 32, 4, Units.Kilograms, true, addedSoFar);
            addedSoFar = await AddKettlebellSet(6, 50, 2, Units.Kilograms, false, addedSoFar);
            // Pounds.
            addedSoFar = await AddKettlebellSet(5, 60, 5, Units.Pounds, true, addedSoFar);
            addedSoFar = await AddKettlebellSet(65, 120, 5, Units.Pounds, false, addedSoFar);
        }
    }

    private static async Task<List<(double, string)>> AddKettlebellSet(double min, double max,
        double step, string units, bool enabled, List<(double, string)> addedSoFar)
    {
        var db = Database.GetConnection();

        for (var weight = min; weight <= max; weight += step)
        {
            // Check we didn't add this one already.
            if (addedSoFar.Contains((weight, units)))
            {
                continue;
            }

            // Get the default colour parameters.
            var (ballColor, hasBands, bandColor) =
                CustomColors.DefaultKettlebellColor(weight, units);

            // Add the kettlebell.
            var kettlebell = new Kettlebell
            {
                Weight = weight,
                Units = units,
                Enabled = enabled,
                BallColor = ballColor.ToHex(),
                HasBands = hasBands,
                BandColor = bandColor?.ToHex(),
            };
            await db.InsertAsync(kettlebell);

            // Remember it.
            addedSoFar.Add((weight, units));
        }

        return addedSoFar;
    }

    /// <summary>
    /// Get the kettlebells.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Kettlebell>> GetAll(string units, bool onlyEnabled = false,
        bool ascending = true)
    {
        return await HeavyThingRepository.GetAll<Kettlebell>(units, onlyEnabled, ascending);
    }
}
