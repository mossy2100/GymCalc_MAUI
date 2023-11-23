using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;

namespace GymCalc.Data;

/// <summary>
/// Methods for CRUD operations on dumbbells.
/// </summary>
public class DumbbellRepository : GymObjectRepository<Dumbbell>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Reference to the single Database object (DI).</param>
    public DumbbellRepository(Database database) : base(database) { }

    /// <inheritdoc/>
    public override async Task InsertDefaults()
    {
        var addedSoFar = new List<(decimal, Units)>();

        // Kilograms.
        addedSoFar = await AddDumbbellSet(1, 10, 1, Units.Kilograms, true, addedSoFar);
        addedSoFar = await AddDumbbellSet(2.5m, 60, 2.5m, Units.Kilograms, true, addedSoFar);

        // Pounds.
        addedSoFar = await AddDumbbellSet(1, 10, 1, Units.Pounds, true, addedSoFar);
        addedSoFar = await AddDumbbellSet(5, 120, 5, Units.Pounds, true, addedSoFar);
    }

    private async Task<List<(decimal, Units)>> AddDumbbellSet(decimal min, decimal max,
        decimal step, Units units, bool enabled, List<(decimal, Units)> addedSoFar)
    {
        for (var weight = min; weight <= max; weight += step)
        {
            // Check we didn't add this one already.
            if (addedSoFar.Contains((weight, units)))
            {
                continue;
            }

            // Add the dumbbell.
            var dumbbell = new Dumbbell
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled,
                Color = "OffBlack"
            };
            await Insert(dumbbell);

            // Remember it.
            addedSoFar.Add((weight, units));
        }

        return addedSoFar;
    }
}
