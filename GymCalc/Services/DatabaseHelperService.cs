using System.ComponentModel;
using GymCalc.Constants;
using GymCalc.Data;

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

    internal GymObjectType GetGymObjectType(string gymObjectTypeName)
    {
        if (!Enum.TryParse<GymObjectType>(gymObjectTypeName, out var gymObjectType))
        {
            throw new InvalidEnumArgumentException("Invalid gym object type name.");
        }
        return gymObjectType;
    }

    /// <summary>
    /// Get the correct repo from the gym object type enum.
    /// </summary>
    /// <param name="gymObjectType"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    internal GymObjectRepository GetRepo(GymObjectType gymObjectType)
    {
        GymObjectRepository repo = gymObjectType switch
        {
            GymObjectType.Bar => _barRepo,
            GymObjectType.Plate => _plateRepo,
            GymObjectType.Dumbbell => _dumbbellRepo,
            GymObjectType.Kettlebell => _kettlebellRepo,
            _ => throw new InvalidEnumArgumentException("Invalid object type."),
        };

        return repo;
    }

    /// <summary>
    /// Get the correct repo from the gym object type name.
    /// </summary>
    /// <param name="gymObjectTypeName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public GymObjectRepository GetRepo(string gymObjectTypeName)
    {
        var gymObjectType = GetGymObjectType(gymObjectTypeName);
        return GetRepo(gymObjectType);
    }
}
