using GymCalc.Enums;
using GymCalc.Models;
using GymCalc.Services;

namespace GymCalc.Repositories;

/// <summary>
/// Methods for CRUD operations on bars.
/// </summary>
public class BarRepository : GymObjectRepository<Bar>
{
    /// <summary>
    /// Default selected bar weight.
    /// </summary>
    internal const decimal DEFAULT_WEIGHT = 20;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Reference to the single Database object (DI).</param>
    public BarRepository(Database database) : base(database) { }

    /// <inheritdoc/>
    protected override Bar Create(decimal weight, EUnits units, bool enabled)
    {
        Bar bar = base.Create(weight, units, enabled);
        bar.Color = "Silver";
        return bar;
    }

    /// <inheritdoc/>
    public override async Task InsertDefaults()
    {
        // Kilograms (common).
        await AddWeights(10, 25, 5, EUnits.Kilograms, true);
        // Pounds (common).
        await AddWeights(15, 55, 10, EUnits.Pounds, true);
    }
}
