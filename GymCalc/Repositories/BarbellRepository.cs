using GymCalc.Enums;
using GymCalc.Models;

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
    public override async Task InsertDefaults()
    {
        // Kilograms (common).
        await AddSet(10, 50, 2.5m, EUnits.Kilograms, true);
        // Kilograms (uncommon).
        await AddSet(52.5m, 70, 2.5m, EUnits.Kilograms, false);
        // Pounds (common).
        await AddSet(20, 110, 5, EUnits.Pounds, true);
        // Pounds (uncommon).
        await AddSet(115, 140, 5, EUnits.Pounds, false);
    }
}
