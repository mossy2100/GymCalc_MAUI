using GymCalc.Data.Models;

namespace GymCalc.Data.Repositories;

/// <summary>
/// Methods for CRUD operations on dumbbells.
/// </summary>
internal static class DumbbellRepository
{
    /// <summary>
    /// Ensure the database table exist and contains some dumbbells.
    /// </summary>
    internal static async Task Initialize()
    {
        var db = Database.GetConnection();

        // Create the table if it doesn't already exist.
        await db.CreateTableAsync<Dumbbell>();

        // Count how many rows there are.
        var n = await db.Table<Dumbbell>().CountAsync();

        // If there aren't any rows, initialize with the defaults.
        if (n == 0)
        {
            var addedSoFar = new List<(double, string)>();

            // Kilograms.
            addedSoFar = await AddDumbbellSet(1, 10, 1, Units.Kilograms, true, addedSoFar);
            addedSoFar = await AddDumbbellSet(2.5, 60, 2.5, Units.Kilograms, true, addedSoFar);

            // Pounds.
            addedSoFar = await AddDumbbellSet(1, 10, 1, Units.Pounds, true, addedSoFar);
            addedSoFar = await AddDumbbellSet(5, 120, 5, Units.Pounds, true, addedSoFar);
        }
    }

    private static async Task<List<(double, string)>> AddDumbbellSet(double min, double max,
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

            // Add the dumbbell.
            var dumbbell = new Dumbbell
            {
                Weight = weight,
                Units = units,
                Enabled = enabled,
            };
            await db.InsertAsync(dumbbell);

            // Remember it.
            addedSoFar.Add((weight, units));
        }

        return addedSoFar;
    }

    /// <summary>
    /// Get the dumbbells.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Dumbbell>> GetAll(string units, bool onlyEnabled = false,
        bool ascending = true)
    {
        return await HeavyThingRepository.GetAll<Dumbbell>(units, onlyEnabled, ascending);
    }
}
