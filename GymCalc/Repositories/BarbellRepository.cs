using Galaxon.Core.Types;
using GymCalc.Constants;
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
        // Function to construct new Barbell objects.
        Func<decimal, Units, bool, Barbell> fn = (weight, units, enabled) =>
            new Barbell
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enabled
            };

        // Kilograms (common).
        await AddSet(10, 50, 2.5m, Units.Kilograms, true, fn);
        // Kilograms (uncommon).
        await AddSet(52.5m, 70, 2.5m, Units.Kilograms, false, fn);
        // Pounds (common).
        await AddSet(20, 110, 5, Units.Pounds, true, fn);
        // Pounds (uncommon).
        await AddSet(115, 140, 5, Units.Pounds, false, fn);
    }
}
