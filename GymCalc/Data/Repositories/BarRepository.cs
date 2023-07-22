using GymCalc.Data.Models;

namespace GymCalc.Data.Repositories;

/// <summary>
/// Methods for CRUD operations on bars.
/// </summary>
internal static class BarRepository
{
    /// <summary>
    /// Default selected bar weight.
    /// </summary>
    internal const double DefaultWeight = 20;

    /// <summary>
    /// Default bars weights to set up on app initialize.
    /// </summary>
    private static readonly double[] _DefaultBars = { 10, 15, 20, 25 };

    /// <summary>
    /// Ensure the database table exist and contains some bars.
    /// </summary>
    internal static async Task Initialize()
    {
        var db = Database.GetConnection();

        // Create the table if it doesn't already exist.
        await db.CreateTableAsync<Bar>();

        // Count how many rows there are.
        var n = await db.Table<Bar>().CountAsync();

        // If there aren't any rows, initialize with the defaults.
        if (n == 0)
        {
            foreach (var weight in _DefaultBars)
            {
                var bar = new Bar
                {
                    Weight = weight,
                    Unit = "kg",
                    Enabled = true,
                };
                await db.InsertAsync(bar);
            }
        }
    }

    /// <summary>
    /// Get the bars.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Bar>> GetAll(bool onlyEnabled = false, bool ascending = true)
    {
        return await HeavyThingRepository.GetAll<Bar>(onlyEnabled, ascending);
    }
}
