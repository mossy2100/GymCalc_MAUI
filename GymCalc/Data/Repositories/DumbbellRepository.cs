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
    internal static async Task InitializeTable()
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
        }
    }

    /// <summary>
    /// Get the dumbbells.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Dumbbell>> GetAll(bool onlyEnabled = false, bool ascending = true)
    {
        await InitializeTable();
        return await HeavyThingRepository.GetAll<Dumbbell>(onlyEnabled, ascending);
    }
}
