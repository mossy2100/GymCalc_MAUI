using GymCalc.Data.Models;

namespace GymCalc.Data.Repositories;

/// <summary>
/// Methods for CRUD operations on bars.
/// </summary>
internal static class BarRepository
{
    public const double DEFAULT_WEIGHT = 20;

    /// <summary>
    /// Default bars.
    /// </summary>
    private static readonly double[] DefaultBars = { 10, 15, 20, 25 };

    /// <summary>
    /// Ensure the database table exist and contains some bars.
    /// </summary>
    public static async Task InitializeTable()
    {
        var db = Database.GetConnection();

        // Create the table if it doesn't already exist.
        await db.CreateTableAsync<Bar>();

        // Count how many rows there are.
        var n = await db.Table<Bar>().CountAsync();

        // If there aren't any rows, initialize with the defaults.
        if (n == 0)
        {
            foreach (var weight in DefaultBars)
            {
                var bar = new Bar
                {
                    Weight = weight,
                    Unit = "kg",
                    Enabled = true
                };
                await db.InsertAsync(bar);
            }
        }
    }
}
