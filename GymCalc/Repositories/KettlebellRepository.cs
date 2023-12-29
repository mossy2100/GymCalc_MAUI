using GymCalc.Enums;
using GymCalc.Models;

namespace GymCalc.Repositories;

/// <summary>
/// Methods for CRUD operations on kettlebells.
/// </summary>
public class KettlebellRepository : GymObjectRepository<Kettlebell>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Reference to the single Database object (DI).</param>
    public KettlebellRepository(Database database) : base(database) { }

    /// <inheritdoc/>
    protected override Kettlebell Create(decimal weight, EUnits units, bool enabled)
    {
        // Construct the new Kettlebell object.
        Kettlebell kettlebell = base.Create(weight, units, enabled);

        // Add the default color parameters.
        (string color, bool hasBands, string? bandColor) = DefaultKettlebellColor(weight, units);
        kettlebell.Color = color;
        kettlebell.HasBands = hasBands;
        kettlebell.BandColor = bandColor;

        return kettlebell;
    }

    /// <inheritdoc/>
    public override async Task InsertDefaults()
    {
        // Kilograms (common).
        await AddSet(4, 32, 4, EUnits.Kilograms, true);
        // Kilograms (uncommon).
        await AddSet(6, 50, 2, EUnits.Kilograms, false);
        // Pounds (common).
        await AddSet(5, 60, 5, EUnits.Pounds, true);
        // Pounds (uncommon).
        await AddSet(65, 120, 5, EUnits.Pounds, false);
    }

    /// <summary>
    /// Get the default colors (as names) for a given kettlebell weight.
    /// </summary>
    /// <param name="weight">The weight of the kettlebell.</param>
    /// <param name="units">The units.</param>
    /// <returns>The default kettlebell color.</returns>
    /// <remarks>
    /// Best image I've found showing a competition kettlebell with black bands:
    /// <see href="https://www.amazon.com/Kettlebell-Kings-Competition-Designed-Repetition/dp/B017WBQSD2?th=1"/>
    /// </remarks>
    private static (string, bool, string?) DefaultKettlebellColor(decimal weight, EUnits units)
    {
        // Determine if the kettlebell has bands and it's number for the color chart.
        bool hasBands;
        decimal n = weight;
        if (units == EUnits.Kilograms)
        {
            hasBands = weight % 4 == 2;
            if (hasBands)
            {
                n -= 2;
            }
            n /= 4;
        }
        else
        {
            hasBands = weight % 10 == 0;
            if (hasBands)
            {
                n -= 5;
            }
            n = (n + 5) / 10;
        }

        // Get ball color.
        string color = n switch
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
            10 => "White",
            11 => "Silver",
            12 => "Gold",
            _ => "Black"
        };

        // Add black bands if it has them.
        string? bandColor = hasBands ? "Black" : null;

        return (color, hasBands, bandColor);
    }
}
