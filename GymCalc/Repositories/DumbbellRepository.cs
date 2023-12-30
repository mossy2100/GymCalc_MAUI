using GymCalc.Enums;
using GymCalc.Models;
using GymCalc.Services;

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
    protected override Dumbbell Create(decimal weight, EUnits units, bool enabled)
    {
        Dumbbell dumbbell = base.Create(weight, units, enabled);
        dumbbell.Color = "Black";
        return dumbbell;
    }

    /// <inheritdoc/>
    public override async Task InsertDefaults()
    {
        // Kilograms (common).
        await AddSet(1, 10, 1, EUnits.Kilograms, true);
        await AddSet(12.5m, 50, 2.5m, EUnits.Kilograms, true);

        // Kilograms (uncommon).
        await AddWeight(7.5m, EUnits.Kilograms, false);
        await AddSet(52.5m, 60, 2.5m, EUnits.Kilograms, false);

        // Pounds (common).
        await AddSet(1, 10, 1, EUnits.Pounds, true);
        await AddSet(15, 120, 5, EUnits.Pounds, true);
    }
}
