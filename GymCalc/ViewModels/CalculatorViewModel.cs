using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Solvers;
using GymCalc.Constants;
using GymCalc.Models;
using GymCalc.Data;

namespace GymCalc.ViewModels;

public class CalculatorViewModel : BaseViewModel
{
    #region Fields

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly DumbbellRepository _dbRepo;

    private readonly KettlebellRepository _kbRepo;

    private string _errorMessage;

    #endregion Fields

    #region Properties

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Bound properties.
    public string MaxWeightText { get; set; }

    public double BarWeight { get; set; }

    public string StartingWeightText { get; set; }

    public bool OneSideOnly { get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Derived properties.
    public ExerciseType SelectedExerciseType { get; set; }

    /// <summary>
    /// Determine the maximum weight from the entry control.
    /// Treat blank as equal to 0.
    /// Any other non-numeric value will return null.
    /// </summary>
    private double? MaxWeight =>
        string.IsNullOrEmpty(StartingWeightText)
            ? 0
            : (double.TryParse(MaxWeightText, out var maxWeight)
                ? maxWeight
                : null);

    /// <summary>
    /// Determine the starting weight from the entry control.
    /// Treat blank as equal to 0.
    /// Any other non-numeric value will return null.
    /// </summary>
    private double? StartingWeight =>
        string.IsNullOrEmpty(StartingWeightText)
            ? 0
            : (double.TryParse(StartingWeightText, out var startingWeight)
                ? startingWeight
                : null);

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Values extracted from user preferences.
    internal static string Units => GymCalc.Constants.Units.GetPreferred();

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Commands.
    public ICommand CalculateCommand { get; private set; }

    /// <summary>
    /// Error message, bindable.
    /// </summary>
    public string ErrorMessage
    {
        get => _errorMessage;

        set => SetProperty(ref _errorMessage, value);
    }

    #endregion Properties

    #region Results

    private List<PlatesResult> _platesResults;

    public List<PlatesResult> PlatesResults
    {
        get => _platesResults;

        set => SetProperty(ref _platesResults, value);
    }

    private bool _platesResultsVisible;

    public bool PlatesResultsVisible
    {
        get => _platesResultsVisible;

        set => SetProperty(ref _platesResultsVisible, value);
    }

    private List<SingleWeightResult> _singleWeightResults;

    public List<SingleWeightResult> SingleWeightResults
    {
        get => _singleWeightResults;

        set => SetProperty(ref _singleWeightResults, value);
    }

    private bool _singleWeightResultsVisible;

    public bool SingleWeightResultsVisible
    {
        get => _singleWeightResultsVisible;

        set => SetProperty(ref _singleWeightResultsVisible, value);
    }

    #endregion Results

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Constructor.
    /// </summary>
    internal CalculatorViewModel(BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dbRepo, KettlebellRepository kbRepo)
    {
        // Keep references to the repositories.
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dbRepo = dbRepo;
        _kbRepo = kbRepo;

        CalculateCommand = new AsyncCommand(async () => await Calculate());
    }

    internal async Task<List<Bar>> GetBars()
    {
        return await _barRepo.GetAll(Units, true);
    }

    #region Validation methods

    private bool ValidateMaxWeight()
    {
        // Check for number greater than 0.
        if (MaxWeight is null or <= 0)
        {
            ErrorMessage = "Please enter a maximum weight greater than 0.";
            return false;
        }

        ErrorMessage = "";
        return true;
    }

    private bool ValidateStartingWeight()
    {
        // Check for number greater than or equal to 0.
        if (StartingWeight is null or < 0)
        {
            ErrorMessage = "Please enter a starting weight greater than or equal to 0.";
            return false;
        }

        ErrorMessage = "";
        return true;
    }

    #endregion Validation methods

    #region Calculations

    private async Task Calculate()
    {
        // Hide current results.
        PlatesResultsVisible = false;
        SingleWeightResultsVisible = false;

        switch (SelectedExerciseType)
        {
            case ExerciseType.Barbell:
                await DoBarbellCalculations();
                break;

            case ExerciseType.Machine:
                await DoMachineCalculations();
                break;

            case ExerciseType.Dumbbell:
                await DoDumbbellCalculations();
                break;

            case ExerciseType.Kettlebell:
                await DoKettlebellCalculations();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(SelectedExerciseType),
                    "Invalid exercise type.");
        }
    }

    private async Task DoBarbellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Calculate and display the results.
        var plates = await _plateRepo.GetAll(Units, true);
        PlatesResults = PlateSolver.CalculateResults(MaxWeight!.Value, BarWeight, true, plates,
            "Plates each end");
        PlatesResultsVisible = true;
    }

    private async Task DoMachineCalculations()
    {
        if (!ValidateMaxWeight() || !ValidateStartingWeight())
        {
            return;
        }

        // Calculate and display the results.
        var plates = await _plateRepo.GetAll(Units, true);
        PlatesResults = PlateSolver.CalculateResults(MaxWeight!.Value, StartingWeight!.Value,
            OneSideOnly, plates, "Plates each side");
        PlatesResultsVisible = true;
    }

    private async Task DoDumbbellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Calculate and display the results.
        var dumbbells = await _dbRepo.GetAll(Units, true);
        SingleWeightResults = SingleWeightSolver.CalculateResults(MaxWeight!.Value, dumbbells);
        SingleWeightResultsVisible = true;
    }

    private async Task DoKettlebellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Calculate and display the results.
        var kettlebells = await _kbRepo.GetAll(Units, true);
        SingleWeightResults = SingleWeightSolver.CalculateResults(MaxWeight!.Value, kettlebells);
        SingleWeightResultsVisible = true;
    }

    #endregion Calculations
}
