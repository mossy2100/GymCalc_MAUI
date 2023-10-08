using Galaxon.Core.Enums;
using GymCalc.Models;
using GymCalc.Constants;

namespace GymCalc.Data;

/// <summary>
/// Methods for CRUD operations on bars.
/// </summary>
public class BarRepository : GymObjectRepository
{
    /// <summary>
    /// Default selected bar weight.
    /// </summary>
    internal const double DEFAULT_WEIGHT = 20;

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
        (55, Units.Pounds, true),
    };

    public BarRepository(Database database) : base(database)
    {
    }

    /// <summary>
    /// Ensure the database table exist and contains some bars.
    /// </summary>
    internal override async Task Initialize()
    {
        await base.Initialize<Bar>();
    }

    internal override async Task InsertDefaults()
    {
        foreach (var (weight, units, enable) in _DefaultBars)
        {
            var bar = new Bar
            {
                Weight = weight,
                Units = units.GetDescription(),
                Enabled = enable,
            };
            await Database.Connection.InsertAsync(bar);
        }
    }

    /// <summary>
    /// Get the bars.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<Bar>> GetAll(Units units = Units.Default, bool? enabled = null,
        bool? ascending = null)
    {
        return await base.GetAll<Bar>(units, enabled, ascending);
    }

    /// <summary>
    /// Get a bar by id.
    /// </summary>
    /// <returns></returns>
    public async Task<Bar> Get(int id)
    {
        return await base.Get<Bar>(id);
    }

    /// <inheritdoc />
    internal override async Task DeleteAll()
    {
        await base.DeleteAll<Bar>();
    }
}
