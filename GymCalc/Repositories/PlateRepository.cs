using GymCalc.Enums;
using GymCalc.Models;

namespace GymCalc.Repositories;

/// <summary>
/// Methods for CRUD operations on plates.
/// </summary>
public class PlateRepository : GymObjectRepository<Plate>
{
    /// <summary>
    /// Default plates.
    /// The key is the plate weight in kilograms. The value is the Enabled flag.
    /// Common plate weights are enabled by default. Less common ones are included but disabled.
    /// </summary>
    private static readonly (decimal, EUnits, bool, string)[] _DefaultPlates =
    [
        // Metric.
        (0.25m, EUnits.Kilograms, false, "Red"),
        (0.5m, EUnits.Kilograms, false, "White"),
        (0.75m, EUnits.Kilograms, false, "Pink"),
        (1m, EUnits.Kilograms, false, "Green"),
        (1.25m, EUnits.Kilograms, true, "Orange"),
        (1.5m, EUnits.Kilograms, false, "Yellow"),
        (2, EUnits.Kilograms, false, "Indigo"),
        (2.5m, EUnits.Kilograms, true, "Red"),
        (5, EUnits.Kilograms, true, "White"),
        (7.5m, EUnits.Kilograms, false, "Pink"),
        (10, EUnits.Kilograms, true, "Green"),
        (12.5m, EUnits.Kilograms, false, "Orange"),
        (15, EUnits.Kilograms, true, "Yellow"),
        (20, EUnits.Kilograms, true, "Indigo"),
        (25, EUnits.Kilograms, true, "Red"),
        // Pounds.
        (0.25m, EUnits.Pounds, false, "Green"),
        (0.5m, EUnits.Pounds, false, "Cyan"),
        (0.75m, EUnits.Pounds, false, "Orange"),
        (1, EUnits.Pounds, false, "White"),
        (1.25m, EUnits.Pounds, true, "Purple"),
        (2.5m, EUnits.Pounds, true, "Green"),
        (5, EUnits.Pounds, true, "Cyan"),
        (10, EUnits.Pounds, true, "White"),
        (15, EUnits.Pounds, true, "Pink"),
        (25, EUnits.Pounds, true, "Green"),
        (35, EUnits.Pounds, true, "Yellow"),
        (45, EUnits.Pounds, true, "Indigo"),
        (55, EUnits.Pounds, true, "Red")
    ];

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Reference to the single Database object (DI).</param>
    public PlateRepository(Database database) : base(database) { }

    /// <inheritdoc/>
    public override async Task InsertDefaults()
    {
        foreach ((decimal weight, EUnits units, bool enabled, string color) in _DefaultPlates)
        {
            // Check that we haven't added this one already.
            Plate? plate = await LoadByWeight(weight, units);

            // If this plate isn't already in the database, construct and insert it.
            if (plate == null)
            {
                // Construct the plate object.
                plate = base.Create(weight, units, enabled);
                plate.Color = color;

                // Add it to the database.
                await Insert(plate);
            }
        }
    }
}
