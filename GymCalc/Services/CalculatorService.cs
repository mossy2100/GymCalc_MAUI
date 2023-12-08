using GymCalc.Constants;
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

    internal List<PlatesResult>? PlatesResults { get; private set; }

    internal List<SingleWeightResult>? SingleWeightResults { get; private set; }

    internal ResultType? SelectedResultType { get; private set; }

    #endregion Properties

    #region Calculation methods

    internal async Task DoBarbellCalculations(BarbellType barbellType, decimal maxWeight,
        decimal barWeight)
    {
        // Calculate the results.
        if (barbellType == BarbellType.PlateLoaded)
        {
            List<Plate> plates = await _plateRepo.LoadSome();
            PlatesResults = PlateSolver.CalculateResults(maxWeight, barWeight, 2, "Plates each end",
                plates);
            SelectedResultType = ResultType.Plates;
        }
        else
        {
            List<Barbell> barbells = await _barbellRepo.LoadSome();
            SingleWeightResults = SingleWeightSolver.CalculateResults(maxWeight, barbells);
            SelectedResultType = ResultType.SingleWeight;
        }
    }

    internal async Task DoMachineCalculations(MachineType machineType, decimal maxWeight,
        decimal startingWeight)
    {
        // Get the available plates.
        List<Plate> plates = await _plateRepo.LoadSome();

        // Determine the number of plate stacks and total starting weight from the machine type.
        int nStacks = machineType == MachineType.Isolateral ? 2 : 1;
        decimal totalStartingWeight = startingWeight * nStacks;
        string eachSideText = machineType == MachineType.Isolateral ? "Plates each side" : "Plates";

        // Calculate the results.
        PlatesResults = PlateSolver.CalculateResults(maxWeight, totalStartingWeight, nStacks,
            eachSideText, plates);
        SelectedResultType = ResultType.Plates;
    }

    internal async Task DoDumbbellCalculations(decimal maxWeight)
    {
        // Calculate the results.
        List<Dumbbell> dumbbells = await _dumbbellRepo.LoadSome();
        SingleWeightResults = SingleWeightSolver.CalculateResults(maxWeight, dumbbells);
        SelectedResultType = ResultType.SingleWeight;
    }

    internal async Task DoKettlebellCalculations(decimal maxWeight)
    {
        // Calculate the results.
        List<Kettlebell> kettlebells = await _kettlebellRepo.LoadSome();
        SingleWeightResults = SingleWeightSolver.CalculateResults(maxWeight, kettlebells);
        SelectedResultType = ResultType.SingleWeight;
    }

    #endregion Calculation methods
}
