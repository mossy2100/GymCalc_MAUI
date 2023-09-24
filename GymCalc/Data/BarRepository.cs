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
    internal const double DefaultWeight = 20;

    /// <summary>
    /// Default bars weights to set up on app initialize.
    /// </summary>
    private static readonly (int, string, bool)[] _DefaultBars =
    {
        // Metric.
        (10, Units.KILOGRAMS, true),
        (15, Units.KILOGRAMS, true),
        (20, Units.KILOGRAMS, true),
        (25, Units.KILOGRAMS, true),
        // US units.
        (15, Units.POUNDS, true),
        (25, Units.POUNDS, true),
        (35, Units.POUNDS, true),
        (45, Units.POUNDS, true),
        (55, Units.POUNDS, true),
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
                Units = units,
                Enabled = enable,
            };
            await Database.Connection.InsertAsync(bar);
        }
    }

    /// <summary>
    /// Get the bars.
    /// </summary>
    /// <returns></returns>
    internal async Task<List<Bar>> GetAll(string units = Units.DEFAULT, bool? enabled = null,
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
