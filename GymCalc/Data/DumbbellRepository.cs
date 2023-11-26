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

        // Kilograms (common).
        await AddSet(1, 10, 1, Units.Kilograms, true, fn);
        await AddSet(2.5m, 60, 2.5m, Units.Kilograms, true, fn);
        // Pounds (common).
        await AddSet(1, 10, 1, Units.Pounds, true, fn);
        await AddSet(5, 120, 5, Units.Pounds, true, fn);
    }
}
