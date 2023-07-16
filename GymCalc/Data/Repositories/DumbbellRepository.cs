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
            // Add light dumbbells, from 1-9 kg.
            double weight;
            for (weight = 1; weight <= 9; weight++)
            {
                var dumbbell = new Dumbbell
                {
                    Weight = weight,
                    Unit = "kg",
                    Enabled = true,
                };
                await db.InsertAsync(dumbbell);
            }

            // Add heavy dumbbells, from 2.5-50 kg.
            for (weight = 2.5; weight <= 50; weight += 2.5)
            {
                // Skip the 5kg, since we added it already.
                if (weight == 5)
                {
                    continue;
                }

                var dumbbell = new Dumbbell
                {
                    Weight = weight,
                    Unit = "kg",
                    Enabled = true,
                };
                await db.InsertAsync(dumbbell);
            }

            // Some other dumbbell weights that are manufactured and are sometimes found in gyms
            // include 11, 12, 13, 14, 16, 18, 22, 24, 26, 28, and 32 kg.
            // However, I think these are less common, and if we provide users with the capability
            // to add their own, it will be enough.
        }
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
