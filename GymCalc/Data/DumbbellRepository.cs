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
        // Function to construct new Dumbbell objects.
        var fn = (decimal weight, Units units, bool enabled) => new Dumbbell()
        {
            Weight = weight,
            Units = units.GetDescription(),
            Enabled = enabled,
            Color = "OffBlack"
        };

        // Kilograms - enabled.
        await AddSet(1, 10, 1, Units.Kilograms, true, fn);
        // Kilograms - disabled.
        await AddSet(2.5m, 60, 2.5m, Units.Kilograms, true, fn);
        // Pounds - enabled.
        await AddSet(1, 10, 1, Units.Pounds, true, fn);
        // Pounds - disabled.
        await AddSet(5, 120, 5, Units.Pounds, true, fn);
    }

    // private async Task<List<(decimal, Units)>> AddDumbbellSet(decimal min, decimal max,
    //     decimal step, Units units, bool enabled, List<(decimal, Units)> addedSoFar)
    // {
    //     for (var weight = min; weight <= max; weight += step)
    //     {
    //         // Check we didn't add this one already.
    //         if (addedSoFar.Contains((weight, units)))
    //         {
    //             continue;
    //         }
    //
    //         // Add the dumbbell.
    //         var dumbbell = new Dumbbell
    //         {
    //             Weight = weight,
    //             Units = units.GetDescription(),
    //             Enabled = enabled,
    //             Color = "OffBlack"
    //         };
    //         await Insert(dumbbell);
    //
    //         // Remember it.
    //         addedSoFar.Add((weight, units));
    //     }
    //
    //     return addedSoFar;
    // }
}
