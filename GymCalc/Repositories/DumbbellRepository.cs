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
        await AddWeights(1, 10, 1, EUnits.Kilograms, true);
        await AddWeights(12.5m, 50, 2.5m, EUnits.Kilograms, true);

        // Kilograms (uncommon).
        await AddWeight(7.5m, EUnits.Kilograms, false);
        await AddWeights(55, 75, 5, EUnits.Kilograms, false);

        // Pounds (common).
        await AddWeights(1, 10, 1, EUnits.Pounds, true);
        await AddWeights(15, 100, 5, EUnits.Pounds, true);

        // Pounds (uncommon).
        await AddWeights(110, 150, 10, EUnits.Pounds, false);
    }
}
