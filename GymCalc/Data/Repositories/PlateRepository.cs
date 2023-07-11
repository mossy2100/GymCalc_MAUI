using GymCalc.Data.Models;

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
    private static readonly Dictionary<double, bool> DefaultPlates = new Dictionary<double, bool>
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
        [25] = true
    };

    /// <summary>
    /// Get the default plate color for a given plate weight.
    /// </summary>
    /// <param name="weight">The weight of the plate in kilograms.</param>
    /// <returns>The default plate color.</returns>
    private static string DefaultPlateColor(double weight)
    {
        while (weight < 5)
        {
            weight *= 10;
        }

        return weight switch
        {
            5 => "#e5e5e5", // white
            7.5 => "#e57ec3", // pink
            10 => "#24b324", // green
            12.5 => "#ff5c26", // orange
            15 => "#f2d024", // yellow
            20 => "#203880", // blue
            25 => "#b3000c", // red
            _ => "#6950b3", // purple
        };
    }

    /// <summary>
    /// Ensure the database table exist and contains some plates.
    /// </summary>
    public static async Task InitializeTable()
    {
        var db = Database.GetConnection();

        // Create the table if it doesn't already exist.
        await db.CreateTableAsync<Plate>();

        // Count how many rows there are.
        var n = await db.Table<Plate>().CountAsync();

        // If there aren't any rows, initialize with the defaults.
        if (n == 0)
        {
            foreach (var (weight, enabled) in DefaultPlates)
            {
                var plate = new Plate
                {
                    Weight = weight,
                    Unit = "kg",
                    Color = DefaultPlateColor(weight),
                    Enabled = enabled
                };
                await db.InsertAsync(plate);
            }
        }
    }

    /// <summary>
    /// Get the available plates.
    /// </summary>
    /// <returns></returns>
    public static List<Plate> GetAllAvailable()
    {
        var db = Database.GetConnection();
        return db.Table<Plate>()
            .Where(p => p.Enabled)
            .OrderByDescending(p => p.Weight)
            .ToListAsync()
            .Result;
    }
}
