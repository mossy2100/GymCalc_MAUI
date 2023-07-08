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
    public static Color DefaultPlateColor(double weight)
    {
        while (weight < 5)
        {
            weight *= 10;
        }

        return weight switch
        {
            25 => Color.FromRgb(0xb3, 0x00, 0x0c), // red
            20 => Color.FromRgb(0x20, 0x38, 0x80), // blue
            15 => Color.FromRgb(0xf2, 0xd0, 0x24), // yellow
            12.5 => Color.FromRgb(0xff, 0x5c, 0x26), // orange
            10 => Color.FromRgb(0x24, 0xb3, 0x24), // green
            7.5 => Color.FromRgb(0xe5, 0x7e, 0xc3), // pink
            5 => Color.FromRgb(0xe5, 0xe5, 0xe5), // white
            _ => Color.FromRgb(0x69, 0x50, 0xb3), // purple
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
                    Color = DefaultPlateColor(weight).ToInt(),
                    Enabled = enabled
                };
                await db.InsertAsync(plate);
            }
        }
    }
}
