using GymCalc.Data.Models;
using GymCalc.Constants;
using GymCalc.Graphics;

namespace GymCalc.Data.Repositories;

/// <summary>
/// Methods for CRUD operations on dumbbells.
/// </summary>
internal class DumbbellRepository : GymObjectRepository
{
    private static DumbbellRepository _instance;

    // Prevent instantiation from outside the class.
    private DumbbellRepository()
    {
    }

    public static DumbbellRepository GetInstance()
    {
        if (_instance == null)
        {
            _instance = new DumbbellRepository();
        }
        return _instance;
    }

    /// <summary>
    /// Ensure the database table exist and contains some bars.
    /// </summary>
    internal override async Task Initialize()
    {
        await base.Initialize<Dumbbell>();
    }

    internal override async Task InsertDefaults()
    {
        var addedSoFar = new List<(double, string)>();

        // Kilograms.
        addedSoFar = await AddDumbbellSet(1, 10, 1, Units.Kilograms, true, addedSoFar);
        addedSoFar = await AddDumbbellSet(2.5, 60, 2.5, Units.Kilograms, true, addedSoFar);

        // Pounds.
        addedSoFar = await AddDumbbellSet(1, 10, 1, Units.Pounds, true, addedSoFar);
        addedSoFar = await AddDumbbellSet(5, 120, 5, Units.Pounds, true, addedSoFar);
    }

    private async Task<List<(double, string)>> AddDumbbellSet(double min, double max,
        double step, string units, bool enabled, List<(double, string)> addedSoFar)
    {
        var db = Database.GetConnection();

        for (var weight = min; weight <= max; weight += step)
        {
            // Check we didn't add this one already.
            if (addedSoFar.Contains((weight, units)))
            {
                continue;
            }

            // Add the dumbbell.
            var dumbbell = new Dumbbell
            {
                Weight = weight,
                Units = units,
                Enabled = enabled,
                Color = "OffBlack",
            };
            await db.InsertAsync(dumbbell);

            // Remember it.
            addedSoFar.Add((weight, units));
        }

        return addedSoFar;
    }

    /// <summary>
    /// Get the dumbbells.
    /// </summary>
    /// <returns></returns>
    public async Task<List<Dumbbell>> GetAll(string units, bool onlyEnabled = false,
        bool ascending = true)
    {
        return await base.GetAll<Dumbbell>(units, onlyEnabled, ascending);
    }

    /// <summary>
    /// Get a dumbbell by id.
    /// </summary>
    /// <returns></returns>
    public async Task<Dumbbell> Get(int id)
    {
        return await base.Get<Dumbbell>(id);
    }

    /// <inheritdoc />
    internal override async Task DeleteAll()
    {
        await base.DeleteAll<Dumbbell>();
    }
}
