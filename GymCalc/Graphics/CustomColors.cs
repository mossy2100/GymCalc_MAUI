using Galaxon.Core.Numbers;
using GymCalc.Constants;

namespace GymCalc.Graphics;

internal static class CustomColors
{
    /// <summary>
    /// Lookup table to map colors to names.
    /// </summary>
    public static readonly Dictionary<string, string> Palette = new Dictionary<string, string>
    {
        { "Red", "#cc0022" },
        { "Orange", "#ff6c00" },
        { "Gold", "#d9a300" },
        { "Yellow", "#f2f224" },
        { "Lime", "#bfff00" },
        { "Green", "#0fb300" },
        { "Cyan", "#80eaff" },
        { "Blue", "#0080ff" },
        { "Indigo", "#0033cc" },
        { "Violet", "#7e5ce5" },
        { "Purple", "#9e19a7" },
        { "Pink", "#ff4cff" },
        { "OffWhite", "#eee" },
        { "Silver", "#c0c0c0" },
        { "Gray", "#808080" },
        { "OffBlack", "#333" },
        { "PaleGray", "#d7d7d7" },
    };

    internal static Color Get(string name)
    {
        return Palette.TryGetValue(name, out var hex) ? Color.Parse(hex) : null;
    }

    /// <summary>
    /// Get the default colors (as names) for a given kettlebell weight.
    ///
    /// Best image I've found showing a competition kettlebell with black bands:
    /// <see href="https://www.amazon.com/Kettlebell-Kings-Competition-Designed-Repetition/dp/B017WBQSD2?th=1" />
    /// </summary>
    /// <param name="weight">The weight of the kettlebell in kilograms.</param>
    /// <returns>The default kettlebell color.</returns>
    internal static (string, bool, string) DefaultKettlebellColor(double weight, string units)
    {
        // Determine if the kettlebell has bands and it's number for the color chart.
        bool hasBands;
        var n = weight;
        if (units == Units.KILOGRAMS)
        {
            hasBands = (weight % 4).FuzzyEquals(2);
            if (hasBands)
            {
                n -= 2;
            }
            n /= 4;
        }
        else
        {
            hasBands = (weight % 10).FuzzyEquals(0);
            if (hasBands)
            {
                n -= 5;
            }
            n = (n + 5) / 10;
        }

        // Get ball color.
        var ballColor = n switch
        {
            1 => "Cyan",
            2 => "Pink",
            3 => "Blue",
            4 => "Yellow",
            5 => "Purple",
            6 => "Green",
            7 => "Orange",
            8 => "Red",
            9 => "Gray",
            10 => "OffWhite",
            11 => "Silver",
            12 => "Gold",
            _ => "OffBlack",
        };

        // Add black bands if it has them.
        var bandColor = hasBands ? "OffBlack" : null;

        return (ballColor, hasBands, bandColor);
    }
}
