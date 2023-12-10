using Galaxon.Core.Types;
using GymCalc.Enums;
using GymCalc.Graphics;
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
    public override async Task InsertDefaults()
    {
        // Function to construct new Kettlebell objects.
        Func<decimal, EUnits, bool, Kettlebell> fn = (weight, units, enabled) =>
        {
            // Get the default color parameters.
            (string ballColor, bool hasBands, string? bandColor) =
                CustomColors.DefaultKettlebellColor(weight, units);

            // Construct the new Kettlebell object.
            return new Kettlebell
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled,
                BallColor = ballColor,
                HasBands = hasBands,
                BandColor = bandColor
            };
        };

        // Kilograms (common).
        await AddSet(4, 32, 4, EUnits.Kilograms, true, fn);
        // Kilograms (uncommon).
        await AddSet(6, 50, 2, EUnits.Kilograms, false, fn);
        // Pounds (common).
        await AddSet(5, 60, 5, EUnits.Pounds, true, fn);
        // Pounds (uncommon).
        await AddSet(65, 120, 5, EUnits.Pounds, false, fn);
    }
}
