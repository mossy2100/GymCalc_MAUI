using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;

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
    public override async Task InsertDefaults()
    {
        // Function to construct new Bar objects.
        Func<decimal, Units, bool, Bar> fn = (weight, units, enabled) => new Bar
        {
            Weight = weight,
            Units = units.GetDescription(),
            Enabled = enabled
        };

        // Kilograms (common).
        await AddSet(10, 25, 5, Units.Kilograms, true, fn);
        // Pounds (common).
        await AddSet(15, 55, 10, Units.Pounds, true, fn);
    }
}
