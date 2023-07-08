using GymCalc.Data.Models;

namespace GymCalc.Data.Repositories;

/// <summary>
/// Methods for CRUD operations on plates.
/// </summary>
internal static class PlateRepository
{
    /// <summary>
    /// Default plates.
    /// </summary>
    private static readonly Dictionary<double, bool> DefaultPlates = new Dictionary<double, bool>
    {
        [25] = true,
        [20] = true,
        [15] = true,
        [12.5] = false,
        [10] = true,
        [7.5] = true,
        [5] = true,
        [2.5] = true,
        [2] = false,
        [1.5] = false,
        [1.25] = true,
        [1] = false,
        [0.75] = false,
        [0.5] = false,
        [0.25] = false
    };

    /// <summary>
    /// Get the default plate color for a given plate weight.
    /// </summary>
    /// <param name="weight">The weight of the plate in kilograms.</param>
    /// <returns>The default plate color.</returns>
    public static string DefaultPlateColor(double weight)
    {
        while (weight < 5)
        {
            weight *= 10;
        }

        return weight switch
        {
            25 => "#b3000c", // red
            20 => "#203880", // blue
            15 => "#f2d024", // yellow
            12.5 => "#ff5c26", // orange
            10 => "#24b324", // green
            7.5 => "#e57ec3", // pink
            5 => "#e5e5e5", // white
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
}
