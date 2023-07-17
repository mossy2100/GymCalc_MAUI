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
            var addedSoFar = new List<double>();
            // Common weights, enabled by default.
            addedSoFar = await AddDumbbellSet(1, 10, 1, true, addedSoFar);
            addedSoFar = await AddDumbbellSet(2.5, 50, 2.5, true, addedSoFar);
            // Less common weights. I may provide buttons to add sets later.
            // addedSoFar = await AddDumbbellSet(11, 14, 1, false, addedSoFar);
            // addedSoFar = await AddDumbbellSet(52.5, 70, 2.5, false, addedSoFar);
            // addedSoFar = await AddDumbbellSet(75, 80, 5, false, addedSoFar);
            // addedSoFar = await AddDumbbellSet(90, 100, 10, false, addedSoFar);
            // Kettlebell weights.
            // addedSoFar = await AddDumbbellSet(4, 48, 2, false, addedSoFar);
        }
    }

    private static async Task<List<double>> AddDumbbellSet(double min, double max, double step, bool enabled,
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

            // Add the dumbbell.
            var dumbbell = new Dumbbell
            {
                Weight = weight,
                Unit = "kg",
                Enabled = enabled,
            };
            await db.InsertAsync(dumbbell);

            // Remember it.
            addedSoFar.Add(weight);
        }

        return addedSoFar;
    }

    /// <summary>
    /// Get the dumbbells.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Dumbbell>> GetAll(bool onlyEnabled = false, bool ascending = true)
    {
        return await HeavyThingRepository.GetAll<Dumbbell>(onlyEnabled, ascending);
    }
}
