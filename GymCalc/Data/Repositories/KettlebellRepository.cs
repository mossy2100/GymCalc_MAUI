using GymCalc.Data.Models;

namespace GymCalc.Data.Repositories;

/// <summary>
/// Methods for CRUD operations on kettlebells.
/// </summary>
internal static class KettlebellRepository
{
    /// <summary>
    /// Get the default color for a given kettlebell weight.
    ///
    /// Best link I've found showing the competition colors with black bands:
    /// <see href="https://www.amazon.com/Kettlebell-Kings-Competition-Designed-Repetition/dp/B017WBQSD2?th=1" />
    /// </summary>
    /// <param name="weight">The weight of the kettlebell in kilograms.</param>
    /// <returns>The default kettlebell color.</returns>
    private static (string, bool) DefaultKettlebellColor(double weight)
    {
        double weightForColor;
        bool hasBlackBands;
        if (weight % 4 == 0)
        {
            weightForColor = weight;
            hasBlackBands = false;
        }
        else
        {
            weightForColor = weight - 2;
            hasBlackBands = true;
        }

        var color = weightForColor switch
        {
            4 => "#7fcfda", // cyan
            8 => "#ff9aff", // pink
            12 => "#0000f5", // blue
            16 => "#ffff54", // yellow
            20 => "#75147c", // purple
            24 => "#00af50", // green
            28 => "#ed7d31", // orange
            32 => "#ea3323", // red
            36 => "#808080", // grey
            40 => "#eee", // off-white
            44 => "#c0c0c0", // silver
            48 => "#ab8300", // gold
            _ => "#222", // off-black
        };

        return (color, hasBlackBands);
    }

    /// <summary>
    /// Ensure the database table exist and contains some kettlebells.
    /// </summary>
    internal static async Task Initialize()
    {
        var db = Database.GetConnection();

        // Create the table if it doesn't already exist.
        await db.CreateTableAsync<Kettlebell>();

        // Count how many rows there are.
        var n = await db.Table<Kettlebell>().CountAsync();

        // If there aren't any rows, initialize with the defaults.
        if (n == 0)
        {
            var addedSoFar = new List<double>();
            // Most common found in gyms, enabled by default.
            addedSoFar = await AddKettlebellSet(4, 32, 4, true, addedSoFar);
            // Less common ones, disabled by default.
            addedSoFar = await AddKettlebellSet(6, 50, 2, false, addedSoFar);
        }
    }

    private static async Task<List<double>> AddKettlebellSet(double min, double max, double step, bool enabled,
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

            // Get the default colour parameters.
            var (color, hasBlackBands) = DefaultKettlebellColor(weight);

            // Add the kettlebell.
            var kettlebell = new Kettlebell
            {
                Weight = weight,
                Unit = "kg",
                Enabled = enabled,
                Color = color,
                HasBlackBands = hasBlackBands
            };
            await db.InsertAsync(kettlebell);

            // Remember it.
            addedSoFar.Add(weight);
        }

        return addedSoFar;
    }

    /// <summary>
    /// Get the kettlebells.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Kettlebell>> GetAll(bool onlyEnabled = false, bool ascending = true)
    {
        return await HeavyThingRepository.GetAll<Kettlebell>(onlyEnabled, ascending);
    }
}
