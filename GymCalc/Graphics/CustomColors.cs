using Galaxon.Core.Numbers;

namespace GymCalc.Graphics;

internal static class CustomColors
{
    internal static Color StainlessSteel = Color.Parse("#ddd");

    internal static Color CastIron = Color.Parse("#222");

    /// <summary>
    /// Get the default plate color for a given plate weight.
    /// </summary>
    /// <param name="weight">The weight of the plate in kilograms.</param>
    /// <returns>The default plate color.</returns>
    internal static string DefaultPlateColor(double weight)
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
    /// Get the default color for a given kettlebell weight.
    ///
    /// Best image I've found showing the competition colors with black bands:
    /// <see href="https://www.amazon.com/Kettlebell-Kings-Competition-Designed-Repetition/dp/B017WBQSD2?th=1" />
    /// </summary>
    /// <param name="weight">The weight of the kettlebell in kilograms.</param>
    /// <returns>The default kettlebell color.</returns>
    internal static (string, bool) DefaultKettlebellColor(double weight)
    {
        double weightForColor;
        bool hasBlackBands;
        if ((weight % 4).FuzzyEquals(2))
        {
            weightForColor = weight - 2;
            hasBlackBands = true;
        }
        else
        {
            weightForColor = weight;
            hasBlackBands = false;
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
            _ => "#222", // cast iron
        };

        return (color, hasBlackBands);
    }
}
