using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;

namespace GymCalc.Data;

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
    /// Default bars weights to set up on app initialize.
    /// </summary>
    private static readonly (int, Units, bool)[] _DefaultBars =
    {
        // Metric.
        (10, Units.Kilograms, true),
        (15, Units.Kilograms, true),
        (20, Units.Kilograms, true),
        (25, Units.Kilograms, true),
        // US units.
        (15, Units.Pounds, true),
        (25, Units.Pounds, true),
        (35, Units.Pounds, true),
        (45, Units.Pounds, true),
        (55, Units.Pounds, true)
    };

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Reference to the single Database object (DI).</param>
    public BarRepository(Database database) : base(database) { }

    /// <inheritdoc/>
    public override async Task InsertDefaults()
    {
        foreach (var (weight, units, enable) in _DefaultBars)
        {
            var bar = new Bar
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enable
            };
            await Insert(bar);
        }
    }
}
