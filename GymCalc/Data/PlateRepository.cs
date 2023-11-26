using Galaxon.Core.Types;
using GymCalc.Constants;
using GymCalc.Models;

namespace GymCalc.Data;

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
    private static readonly (decimal, Units, bool, string)[] _DefaultPlates =
    {
        // Metric.
        (0.25m, Units.Kilograms, false, "Red"),
        (0.5m, Units.Kilograms, false, "OffWhite"),
        (0.75m, Units.Kilograms, false, "Pink"),
        (1m, Units.Kilograms, false, "Green"),
        (1.25m, Units.Kilograms, true, "Orange"),
        (1.5m, Units.Kilograms, false, "Yellow"),
        (2, Units.Kilograms, false, "Indigo"),
        (2.5m, Units.Kilograms, true, "Red"),
        (5, Units.Kilograms, true, "OffWhite"),
        (7.5m, Units.Kilograms, false, "Pink"),
        (10, Units.Kilograms, true, "Green"),
        (12.5m, Units.Kilograms, false, "Orange"),
        (15, Units.Kilograms, true, "Yellow"),
        (20, Units.Kilograms, true, "Indigo"),
        (25, Units.Kilograms, true, "Red"),
        // Pounds.
        (0.25m, Units.Pounds, false, "Green"),
        (0.5m, Units.Pounds, false, "Cyan"),
        (0.75m, Units.Pounds, false, "Pink"),
        (1, Units.Pounds, false, "OffWhite"),
        (1.25m, Units.Pounds, true, "Orange"),
        (2.5m, Units.Pounds, true, "Green"),
        (5, Units.Pounds, true, "Cyan"),
        (10, Units.Pounds, true, "OffWhite"),
        (15, Units.Pounds, true, "Purple"),
        (25, Units.Pounds, true, "Green"),
        (35, Units.Pounds, true, "Yellow"),
        (45, Units.Pounds, true, "Indigo"),
        (55, Units.Pounds, true, "Red")
    };

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database">Reference to the single Database object (DI).</param>
    public PlateRepository(Database database) : base(database) { }

    /// <inheritdoc/>
    public override async Task InsertDefaults()
    {
        foreach (var (weight, units, enabled, color) in _DefaultPlates)
        {
            // Check that we haven't added this one already.
            var plate = await LoadOneByWeight(weight, units);

            // If this plate isn't already in the database, construct and insert it.
            if (plate == null)
            {
                plate = new Plate
                {
                    Weight = weight,
                    Units = units.GetDescription(),
                    Enabled = enabled,
                    Color = color
                };
                await Insert(plate);
            }
        }
    }
}
