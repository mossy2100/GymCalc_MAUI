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
    private static readonly (int, string, bool)[] _DefaultBars =
    {
        // Metric.
        (10, Units.Kilograms, true),
        (15, Units.Kilograms, true),
        (20, Units.Kilograms, true),
        (25, Units.Kilograms, true),
        // US units.
        (15, Units.Pounds, true),
        (25, Units.Pounds, true),
        (35, Units.Pounds, true),
        (45, Units.Pounds, true),
        (55, Units.Pounds, true),
    };

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
            foreach (var (weight, units, enable) in _DefaultBars)
            {
                var bar = new Bar
                {
                    Weight = weight,
                    Units = units,
                    Enabled = enable,
                };
                await db.InsertAsync(bar);
            }
        }
    }

    /// <summary>
    /// Get the bars.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Bar>> GetAll(string units, bool onlyEnabled = false,
        bool ascending = true)
    {
        return await HeavyThingRepository.GetAll<Bar>(units, onlyEnabled, ascending);
    }
}
