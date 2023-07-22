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
    private static readonly Dictionary<double, bool> _DefaultPlates = new ()
    {
        [0.25] = false,
        [0.5] = false,
        [0.75] = false,
        [1] = false,
        [1.25] = true,
        [1.5] = false,
        [2] = false,
        [2.5] = true,
        [5] = true,
        [7.5] = true,
        [10] = true,
        [12.5] = false,
        [15] = true,
        [20] = true,
        [25] = true,
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
            foreach (var (weight, enabled) in _DefaultPlates)
            {
                var plate = new Plate
                {
                    Weight = weight,
                    Unit = "kg",
                    Color = CustomColors.DefaultPlateColor(weight),
                    Enabled = enabled,
                };
                await db.InsertAsync(plate);
            }
        }
    }

    /// <summary>
    /// Get the plates.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Plate>> GetAll(bool onlyEnabled = false, bool ascending = true)
    {
        return await HeavyThingRepository.GetAll<Plate>(onlyEnabled, ascending);
    }
}
