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

    private string _maxWeightText;

    public string MaxWeightText
    {
        get => _maxWeightText;

        set => SetProperty(ref _maxWeightText, value);
    }

    private double _barWeight;

    public double BarWeight
    {
        get => _barWeight;

        set => SetProperty(ref _barWeight, value);
    }

    private List<double> _barWeights;

    public List<double> BarWeights
    {
        get => _barWeights;

        set => SetProperty(ref _barWeights, value);
    }

    private string _startingWeightText;

    public string StartingWeightText
    {
        get => _startingWeightText;

        set => SetProperty(ref _startingWeightText, value);
    }

    private bool _oneSideOnly;

    public bool OneSideOnly
    {
        get => _oneSideOnly;

        set => SetProperty(ref _oneSideOnly, value);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Derived properties.
    public ExerciseType SelectedExerciseType { get; set; }

    /// <summary>
    /// Determine the maximum weight from the entry control.
    /// Treat blank as equal to 0.
    /// Any other non-numeric value will return null.
    /// </summary>
    private double? MaxWeight =>
        string.IsNullOrEmpty(MaxWeightText)
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
    // internal static string Units => GymCalc.Constants.Units.GetPreferred();

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

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    public CalculatorViewModel(BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dbRepo, KettlebellRepository kbRepo)
    {
        // Keep references to the repositories.
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dbRepo = dbRepo;
        _kbRepo = kbRepo;

        // Create commands.
        CalculateCommand = new AsyncCommand(async () => await Calculate());
    }

    #endregion Constructor

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

        // Do the calculations based on the selected exercise type.
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
        var plates = await _plateRepo.GetAll(enabled: true, ascending: true);
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
        var plates = await _plateRepo.GetAll(enabled: true, ascending: true);
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
        var dumbbells = await _dbRepo.GetAll(enabled: true, ascending: true);
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
        var kettlebells = await _kbRepo.GetAll(enabled: true, ascending: true);
        SingleWeightResults = SingleWeightSolver.CalculateResults(MaxWeight!.Value, kettlebells);
        SingleWeightResultsVisible = true;
    }

    #endregion Calculations

    #region Database stuff

    internal async Task InitializeDatabase()
    {
        var barTask = _barRepo.Initialize();
        var plateTask = _plateRepo.Initialize();
        var dumbbellTask = _dbRepo.Initialize();
        var kettlebellTask = _kbRepo.Initialize();
        await Task.WhenAll(new Task[] { barTask, plateTask, dumbbellTask, kettlebellTask });
    }

    internal async Task<List<Bar>> GetBars()
    {
        return await _barRepo.GetAll(enabled: true, ascending: true);
    }

    /// <summary>
    /// Reset the bar weight picker items.
    /// </summary>
    internal async Task ResetBarWeightPicker()
    {
        // Get the current selected value.
        // var initialSelectedIndex = BarWeight.SelectedIndex;
        // var initialSelectedValue = BarWeight;

        // Reset the picker items.
        var bars = await GetBars();
        BarWeights = bars.Select(b => b.Weight).ToList();

        // BarWeight.Items.Clear();
        // BarWeight.SelectedIndex = -1;

        // // Initialise the items in the bar weight picker.
        // var i = 0;
        // var valueSelected = false;
        // foreach (var bar in bars)
        // {
        //     // Add the picker item.
        //     var weightString = bar.Weight.ToString(CultureInfo.InvariantCulture);
        //     BarWeight.Items.Add(weightString);
        //
        //     // Try to select the same weight that was selected before.
        //     if (!valueSelected && weightString == initialSelectedValue)
        //     {
        //         BarWeight.SelectedIndex = i;
        //         valueSelected = true;
        //     }
        //
        //     i++;
        // }

        // If the original selected bar weight is no longer present, try to select the default.
        // if (!valueSelected)
        // {
        //     var weightString = BarRepository.DefaultWeight.ToString(CultureInfo.InvariantCulture);
        //     for (i = 0; i < BarWeight.Items.Count; i++)
        //     {
        //         // Default selection.
        //         if (BarWeight.Items[i] == weightString)
        //         {
        //             BarWeight.SelectedIndex = i;
        //             valueSelected = true;
        //             break;
        //         }
        //     }
        // }
        //
        // // If no bar weight has been selected yet, select the first one.
        // if (!valueSelected)
        // {
        //     BarWeight.SelectedIndex = 0;
        // }
    }

    #endregion Database stuff
}
