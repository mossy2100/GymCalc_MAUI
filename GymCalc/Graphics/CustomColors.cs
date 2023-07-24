using Galaxon.Core.Numbers;

namespace GymCalc.Graphics;

internal static class CustomColors
{
    /// <summary>
    /// Color palette for graphics.
    /// </summary>
    public static readonly Color Red = Color.Parse("#b3000c");

    public static readonly Color Orange = Color.Parse("#ff6c00");

    public static readonly Color Gold = Color.Parse("#d9a300");

    public static readonly Color Yellow = Color.Parse("#f2f224");

    public static readonly Color Lime = Color.Parse("#bfff00");

    public static readonly Color Green = Color.Parse("#0fb300");

    public static readonly Color Cyan = Color.Parse("#73d8e6");

    public static readonly Color Blue = Color.Parse("#2e4eec");

    public static readonly Color Indigo = Color.Parse("#203880");

    public static readonly Color Violet = Color.Parse("#7052cc");

    public static readonly Color Purple = Color.Parse("#75147c");

    public static readonly Color Pink = Color.Parse("#ff9aff");

    public static readonly Color OffWhite = Color.Parse("#eee");

    public static readonly Color PaleGray = Color.Parse("#d7d7d7");

    public static readonly Color Silver = Color.Parse("#c0c0c0");

    public static readonly Color Gray = Color.Parse("#808080");

    public static readonly Color OffBlack = Color.Parse("#333");

    /// <summary>
    /// Get the default color for a given kettlebell weight.
    ///
    /// Best image I've found showing a competition kettlebell with black bands:
    /// <see href="https://www.amazon.com/Kettlebell-Kings-Competition-Designed-Repetition/dp/B017WBQSD2?th=1" />
    /// </summary>
    /// <param name="weight">The weight of the kettlebell in kilograms.</param>
    /// <returns>The default kettlebell color.</returns>
    internal static (Color, bool, Color) DefaultKettlebellColor(double weight, string units)
    {
        Color ballColor;
        bool hasBands;

        var weightForColor = weight;

        if (units == Units.Kilograms)
        {
            hasBands = (weight % 4).FuzzyEquals(2);
            if (hasBands)
            {
                weightForColor -= 2;
            }

            // Get ball color.
            ballColor = weightForColor switch
            {
                4 => Cyan,
                8 => Pink,
                12 => Blue,
                16 => Yellow,
                20 => Purple,
                24 => Green,
                28 => Orange,
                32 => Red,
                36 => Gray,
                40 => OffWhite,
                44 => Silver,
                48 => Gold,
                _ => OffBlack,
            };
        }
        else
        {
            hasBands = (weight % 10).FuzzyEquals(0);
            if (hasBands)
            {
                weightForColor -= 5;
            }

            // Get ball color.
            ballColor = weightForColor switch
            {
                5 => Cyan,
                15 => Pink,
                25 => Blue,
                35 => Yellow,
                45 => Purple,
                55 => Green,
                65 => Orange,
                75 => Red,
                85 => Gray,
                95 => OffWhite,
                105 => Silver,
                115 => Gold,
                _ => OffBlack,
            };
        }

        // Get band color.
        var bandColor = hasBands ? OffBlack : null;

        return (ballColor, hasBands, bandColor);
    }
}
