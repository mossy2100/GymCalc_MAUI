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
            var addedSoFar = new List<double>();
            // Most common found in gyms, enabled by default.
            addedSoFar = await AddKettlebellSet(4, 32, 4, true, addedSoFar);
            // Less common ones, disabled by default.
            addedSoFar = await AddKettlebellSet(6, 50, 2, false, addedSoFar);
        }
    }

    private static async Task<List<double>> AddKettlebellSet(double min, double max, double step, bool enabled,
        List<double> addedSoFar)
    {
        var db = Database.GetConnection();

        for (var weight = min; weight <= max; weight += step)
        {
            // Check we didn't add this one already.
            if (addedSoFar.Contains(weight))
            {
                continue;
            }

            // Get the default colour parameters.
            var (ballColor, hasBands, bandColor) = CustomColors.DefaultKettlebellColor(weight);

            // Add the kettlebell.
            var kettlebell = new Kettlebell
            {
                Weight = weight,
                Unit = "kg",
                Enabled = enabled,
                BallColor = ballColor.ToHex(),
                HasBands = hasBands,
                BandColor = bandColor?.ToHex(),
            };
            await db.InsertAsync(kettlebell);

            // Remember it.
            addedSoFar.Add(weight);
        }

        return addedSoFar;
    }

    /// <summary>
    /// Get the kettlebells.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Kettlebell>> GetAll(bool onlyEnabled = false, bool ascending = true)
    {
        return await HeavyThingRepository.GetAll<Kettlebell>(onlyEnabled, ascending);
    }
}
