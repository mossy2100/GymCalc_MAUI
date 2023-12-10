using Galaxon.Core.Types;
using GymCalc.Enums;
using GymCalc.Models;

namespace GymCalc.Repositories;

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
        Func<decimal, EUnits, bool, Dumbbell> fn = (weight, units, enabled) =>
            new Dumbbell
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled,
                Color = "OffBlack"
            };

        // Kilograms (common).
        await AddSet(1, 10, 1, EUnits.Kilograms, true, fn);
        await AddSet(12.5m, 50, 2.5m, EUnits.Kilograms, true, fn);
        // Kilograms (uncommon).
        await AddWeight(7.5m, EUnits.Kilograms, false, fn);
        await AddSet(52.5m, 60, 2.5m, EUnits.Kilograms, false, fn);
        // Pounds (common).
        await AddSet(1, 10, 1, EUnits.Pounds, true, fn);
        await AddSet(15, 120, 5, EUnits.Pounds, true, fn);
    }
}
