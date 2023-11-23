using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;

namespace GymCalc.Data;

/// <summary>
/// Methods for CRUD operations on fixed-weight barbells.
/// </summary>
public class BarbellRepository : GymObjectRepository<Barbell>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Reference to the single Database object (DI).</param>
    public BarbellRepository(Database database) : base(database) { }

    /// <inheritdoc/>
    public override async Task InsertDefaults()
    {
        var addedSoFar = new List<(decimal, Units)>();

        // Kilograms.
        addedSoFar = await AddBarbellSet(7.5m, 70, 2.5m, Units.Kilograms, true, addedSoFar);

        // Pounds.
        addedSoFar = await AddBarbellSet(20, 140, 5, Units.Pounds, true, addedSoFar);
    }

    /// <summary>
    /// Add a set of barbells to the database.
    /// </summary>
    /// <param name="min">The minimum weight.</param>
    /// <param name="max">The maximum weight.</param>
    /// <param name="step">The difference between each weight.</param>
    /// <param name="units">The mass units.</param>
    /// <param name="enabled">If they should be enabled by default.</param>
    /// <param name="addedSoFar">The barbells added to so far, so we can avoid duplicates.</param>
    /// <returns></returns>
    private async Task<List<(decimal, Units)>> AddBarbellSet(decimal min, decimal max,
        decimal step, Units units, bool enabled, List<(decimal, Units)> addedSoFar)
    {
        for (var weight = min; weight <= max; weight += step)
        {
            // Check we didn't add this one already.
            if (addedSoFar.Contains((weight, units)))
            {
                continue;
            }

            // Add the barbell.
            var barbell = new Barbell
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled
            };
            await Insert(barbell);

            // Remember it.
            addedSoFar.Add((weight, units));
        }

        return addedSoFar;
    }
}
