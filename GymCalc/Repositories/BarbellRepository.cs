using GymCalc.Enums;
using GymCalc.Models;
using GymCalc.Services;

namespace GymCalc.Repositories;

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
    public override Barbell Create()
    {
        return new Barbell();
    }

    /// <inheritdoc/>
    protected override Barbell Create(decimal weight, EUnits units, bool enabled)
    {
        Barbell barbell = base.Create(weight, units, enabled);
        barbell.Color = "Black";
        return barbell;
    }

    /// <inheritdoc/>
    public override async Task InsertDefaults()
    {
        // Kilograms (common).
        await AddWeights(10, 50, 2.5m, EUnits.Kilograms, true);
        // Kilograms (uncommon).
        await AddWeights(52.5m, 70, 2.5m, EUnits.Kilograms, false);
        // Pounds (common).
        await AddWeights(20, 110, 5, EUnits.Pounds, true);
        // Pounds (uncommon).
        await AddWeights(115, 140, 5, EUnits.Pounds, false);
    }
}
