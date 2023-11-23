using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Graphics;
using GymCalc.Models;

namespace GymCalc.Data;

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
        var addedSoFar = new List<(decimal, Units)>();
        // Kilograms.
        addedSoFar = await AddKettlebellSet(4, 32, 4, Units.Kilograms, true, addedSoFar);
        addedSoFar = await AddKettlebellSet(6, 50, 2, Units.Kilograms, false, addedSoFar);
        // Pounds.
        addedSoFar = await AddKettlebellSet(5, 60, 5, Units.Pounds, true, addedSoFar);
        addedSoFar = await AddKettlebellSet(65, 120, 5, Units.Pounds, false, addedSoFar);
    }

    private async Task<List<(decimal, Units)>> AddKettlebellSet(decimal min, decimal max,
        decimal step, Units units, bool enabled, List<(decimal, Units)> addedSoFar)
    {
        for (var weight = min; weight <= max; weight += step)
        {
            // Check we didn't add this one already.
            if (addedSoFar.Contains((weight, units)))
            {
                continue;
            }

            // Get the default color parameters.
            var (ballColor, hasBands, bandColor) =
                CustomColors.DefaultKettlebellColor(weight, units);

            // Add the kettlebell.
            var kettlebell = new Kettlebell
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled,
                BallColor = ballColor,
                HasBands = hasBands,
                BandColor = bandColor
            };
            await Insert(kettlebell);

            // Remember it.
            addedSoFar.Add((weight, units));
        }

        return addedSoFar;
    }
}
