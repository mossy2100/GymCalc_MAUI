using GymCalc.Enums;
using GymCalc.Models;
using GymCalc.Repositories;

namespace GymCalc.Services;

public class CalculatorService
{
    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    public CalculatorService(PlateRepository plateRepo, BarbellRepository barbellRepo,
        DumbbellRepository dumbbellRepo, KettlebellRepository kettlebellRepo)
    {
        // Keep references to the repositories.
        _plateRepo = plateRepo;
        _barbellRepo = barbellRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;
    }

    #endregion Constructor

    #region Dependencies

    private readonly PlateRepository _plateRepo;

    private readonly BarbellRepository _barbellRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    #endregion Dependencies

    #region Properties

    internal EExerciseType? ExerciseType { get; set; }

    internal decimal? MaxWeight { get; set; }

    internal EBarbellType? BarbellType { get; set; }

    internal decimal? BarWeight { get; set; }

    internal EMachineType? MachineType { get; set; }

    internal decimal? StartingWeight { get; set; }

    internal EResultType? ResultType { get; private set; }

    internal List<PlatesResult>? PlatesResults { get; private set; }

    internal List<SingleWeightResult>? SingleWeightResults { get; private set; }

    #endregion Properties

    #region Calculation methods

    /// <summary>
    /// Reset the calculator in preparation for a new calculation.
    /// </summary>
    private void Reset()
    {
        ExerciseType = null;
        MaxWeight = null;
        BarbellType = null;
        BarWeight = null;
        MachineType = null;
        StartingWeight = null;
        ResultType = null;
        PlatesResults = null;
        SingleWeightResults = null;
    }

    internal async Task DoBarbellCalculations(EBarbellType barbellType, decimal maxWeight,
        decimal barWeight)
    {
        // Update parameters.
        Reset();
        ExerciseType = EExerciseType.Barbell;
        MaxWeight = maxWeight;
        BarbellType = barbellType;
        BarWeight = barWeight;

        // Calculate the results.
        if (barbellType == EBarbellType.PlateLoaded)
        {
            PlatesResults = await PlateSolver.CalculateResults(maxWeight, barWeight, 2,
                "Plates each end", _plateRepo);
            ResultType = EResultType.Plates;
        }
        else
        {
            SingleWeightResults =
                await SingleWeightSolver.CalculateResults(maxWeight, _barbellRepo);
            ResultType = EResultType.SingleWeight;
        }
    }

    internal async Task DoMachineCalculations(EMachineType machineType, decimal maxWeight,
        decimal startingWeight)
    {
        // Update parameters.
        Reset();
        ExerciseType = EExerciseType.Machine;
        MaxWeight = maxWeight;
        MachineType = machineType;
        StartingWeight = startingWeight;

        // Determine the number of plate stacks and total starting weight from the machine type.
        int nStacks = machineType == EMachineType.Isolateral ? 2 : 1;
        decimal totalStartingWeight = startingWeight * nStacks;
        string eachSideText =
            machineType == EMachineType.Isolateral ? "Plates each side" : "Plates";

        // Calculate the results.
        PlatesResults = await PlateSolver.CalculateResults(maxWeight, totalStartingWeight, nStacks,
            eachSideText, _plateRepo);
        ResultType = EResultType.Plates;
    }

    internal async Task DoDumbbellCalculations(decimal maxWeight)
    {
        // Update parameters.
        Reset();
        ExerciseType = EExerciseType.Dumbbell;
        MaxWeight = maxWeight;

        // Calculate the results.
        SingleWeightResults = await SingleWeightSolver.CalculateResults(maxWeight, _dumbbellRepo);
        ResultType = EResultType.SingleWeight;
    }

    internal async Task DoKettlebellCalculations(decimal maxWeight)
    {
        // Update parameters.
        Reset();
        ExerciseType = EExerciseType.Kettlebell;
        MaxWeight = maxWeight;

        // Calculate the results.
        SingleWeightResults = await SingleWeightSolver.CalculateResults(maxWeight, _kettlebellRepo);
        ResultType = EResultType.SingleWeight;
    }

    #endregion Calculation methods
}
