using GymCalc.Data;
using GymCalc.Models;

namespace GymCalc.Services;

public class DatabaseHelperService
{
    // ---------------------------------------------------------------------------------------------
    // Dependencies.

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="dumbbellRepo"></param>
    /// <param name="kettlebellRepo"></param>
    public DatabaseHelperService(BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dumbbellRepo, KettlebellRepository kettlebellRepo)
    {
        // Dependencies.
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Get the correct repo from the gym object type enum.
    /// </summary>
    /// <param name="gymObjectTypeName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal GymObjectRepository GetRepo(string gymObjectTypeName)
    {
        GymObjectRepository repo = gymObjectTypeName switch
        {
            nameof(Bar) => _barRepo,
            nameof(Plate) => _plateRepo,
            nameof(Dumbbell) => _dumbbellRepo,
            nameof(Kettlebell) => _kettlebellRepo,
            _ => throw new ArgumentOutOfRangeException(nameof(gymObjectTypeName),
                "Invalid object type."),
        };

        return repo;
    }
}
