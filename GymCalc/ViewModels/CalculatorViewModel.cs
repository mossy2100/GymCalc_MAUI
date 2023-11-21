using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using Galaxon.Core.Enums;
using Galaxon.Core.Exceptions;
using GymCalc.Constants;
using GymCalc.Data;
using GymCalc.Models;
using GymCalc.Shared;
using GymCalc.Solvers;

namespace GymCalc.ViewModels;

public class CalculatorViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public CalculatorViewModel(BarRepository barRepo, PlateRepository plateRepo,
        BarbellRepository barbellRepo, DumbbellRepository dumbbellRepo,
        KettlebellRepository kettlebellRepo)
    {
        // Keep references to the repositories.
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _barbellRepo = barbellRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;

        // Create commands.
        CalculateCommand = new AsyncCommand(Calculate);
        MachineTypeChangedCommand = new Command(MachineTypeChanged);
        PercentSelectedCommand = new Command<string>(PercentSelected);
    }

    #endregion Constructors

    // ---------------------------------------------------------------------------------------------
    #region Dependencies

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly BarbellRepository _barbellRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    #endregion Dependencies

    // ---------------------------------------------------------------------------------------------
    #region Bindable properties

    // ---------------------------------------------------------------------------------------------
    private string _maxWeightText;

    public string MaxWeightText
    {
        get => _maxWeightText;

        set => SetProperty(ref _maxWeightText, value);
    }

    // ---------------------------------------------------------------------------------------------
    private decimal _barWeight;

    public decimal BarWeight
    {
        get => _barWeight;

        set => SetProperty(ref _barWeight, value);
    }

    // ---------------------------------------------------------------------------------------------
    private List<decimal> _barWeights;

    public List<decimal> BarWeights
    {
        get => _barWeights;

        set => SetProperty(ref _barWeights, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _startingWeightText;

    public string StartingWeightText
    {
        get => _startingWeightText;

        set => SetProperty(ref _startingWeightText, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _startingWeightLabel;

    public string StartingWeightLabel
    {
        get => _startingWeightLabel;

        set => SetProperty(ref _startingWeightLabel, value);
    }

    // ---------------------------------------------------------------------------------------------
    private MachineType _machineType = MachineType.Bilateral;

    public MachineType MachineType
    {
        get => _machineType;

        set => SetProperty(ref _machineType, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _maxWeightUnits;

    public string MaxWeightUnits
    {
        get => _maxWeightUnits;

        set => SetProperty(ref _maxWeightUnits, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _barWeightUnits;

    public string BarWeightUnits
    {
        get => _barWeightUnits;

        set => SetProperty(ref _barWeightUnits, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _startingWeightUnits;

    public string StartingWeightUnits
    {
        get => _startingWeightUnits;

        set => SetProperty(ref _startingWeightUnits, value);
    }

    // ---------------------------------------------------------------------------------------------
    private ExerciseType _selectedExerciseType;

    public ExerciseType SelectedExerciseType
    {
        get => _selectedExerciseType;

        set => SetProperty(ref _selectedExerciseType, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _errorMessage;

    public string ErrorMessage
    {
        get => _errorMessage;

        set => SetProperty(ref _errorMessage, value);
    }

    // ---------------------------------------------------------------------------------------------
    private int _selectedPercent;

    public int SelectedPercent
    {
        get => _selectedPercent;

        set => SetProperty(ref _selectedPercent, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _percentButtonVisualState50;

    public string PercentButtonVisualState50
    {
        get => _percentButtonVisualState50;

        set => SetProperty(ref _percentButtonVisualState50, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _percentButtonVisualState60;

    public string PercentButtonVisualState60
    {
        get => _percentButtonVisualState60;

        set => SetProperty(ref _percentButtonVisualState60, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _percentButtonVisualState70;

    public string PercentButtonVisualState70
    {
        get => _percentButtonVisualState70;

        set => SetProperty(ref _percentButtonVisualState70, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _percentButtonVisualState80;

    public string PercentButtonVisualState80
    {
        get => _percentButtonVisualState80;

        set => SetProperty(ref _percentButtonVisualState80, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _percentButtonVisualState90;

    public string PercentButtonVisualState90
    {
        get => _percentButtonVisualState90;

        set => SetProperty(ref _percentButtonVisualState90, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string _percentButtonVisualState100;

    public string PercentButtonVisualState100
    {
        get => _percentButtonVisualState100;

        set => SetProperty(ref _percentButtonVisualState100, value);
    }

    // ---------------------------------------------------------------------------------------------
    private List<PlatesResult> _platesResults;

    public List<PlatesResult> PlatesResults
    {
        get => _platesResults;

        set => SetProperty(ref _platesResults, value);
    }

    // ---------------------------------------------------------------------------------------------
    private PlatesResult _selectedPlatesResult;

    public PlatesResult SelectedPlatesResult
    {
        get => _selectedPlatesResult;

        set => SetProperty(ref _selectedPlatesResult, value);
    }

    // ---------------------------------------------------------------------------------------------
    private bool _platesResultVisible;

    public bool PlatesResultVisible
    {
        get => _platesResultVisible;

        set => SetProperty(ref _platesResultVisible, value);
    }

    // ---------------------------------------------------------------------------------------------
    private List<SingleWeightResult> _singleWeightResults;

    public List<SingleWeightResult> SingleWeightResults
    {
        get => _singleWeightResults;

        set => SetProperty(ref _singleWeightResults, value);
    }

    // ---------------------------------------------------------------------------------------------
    private SingleWeightResult _selectedSingleWeightResult;

    public SingleWeightResult SelectedSingleWeightResult
    {
        get => _selectedSingleWeightResult;

        set => SetProperty(ref _selectedSingleWeightResult, value);
    }

    // ---------------------------------------------------------------------------------------------
    private bool _singleWeightResultVisible;

    public bool SingleWeightResultVisible
    {
        get => _singleWeightResultVisible;

        set => SetProperty(ref _singleWeightResultVisible, value);
    }

    // ---------------------------------------------------------------------------------------------
    private bool _resultsVisible;

    public bool ResultsVisible
    {
        get => _resultsVisible;

        set => SetProperty(ref _resultsVisible, value);
    }

    // ---------------------------------------------------------------------------------------------
    private ExerciseType _resultsExerciseType;

    public ExerciseType ResultsExerciseType
    {
        get => _resultsExerciseType;

        set => SetProperty(ref _resultsExerciseType, value);
    }

    #endregion Bindable properties

    // ---------------------------------------------------------------------------------------------
    #region Calculated properties

    /// <summary>
    /// Determine the maximum weight from the entry control.
    /// Treat blank as equal to 0.
    /// Any other non-numeric value will return null.
    /// </summary>
    private decimal? MaxWeight =>
        string.IsNullOrEmpty(MaxWeightText)
            ? 0
            : decimal.TryParse(MaxWeightText, out var maxWeight)
                ? maxWeight
                : null;

    /// <summary>
    /// Determine the starting weight from the entry control.
    /// Treat blank as equal to 0.
    /// Any other non-numeric value will return null.
    /// </summary>
    private decimal? StartingWeight =>
        string.IsNullOrEmpty(StartingWeightText)
            ? 0
            : decimal.TryParse(StartingWeightText, out var startingWeight)
                ? startingWeight
                : null;

    #endregion

    // ---------------------------------------------------------------------------------------------
    #region Commands

    public ICommand CalculateCommand { get; init; }

    public ICommand MachineTypeChangedCommand { get; init; }

    public ICommand PercentSelectedCommand { get; init; }

    #endregion Commands

    // ---------------------------------------------------------------------------------------------
    #region Initialization stuff

    internal async Task InitializeDatabase()
    {
        var barTask = _barRepo.Initialize();
        var plateTask = _plateRepo.Initialize();
        var barbellTask = _barbellRepo.Initialize();
        var dumbbellTask = _dumbbellRepo.Initialize();
        var kettlebellTask = _kettlebellRepo.Initialize();
        await Task.WhenAll(barTask, plateTask, barbellTask, dumbbellTask, kettlebellTask);
    }

    /// <summary>
    /// Reset the bar weight picker items.
    /// </summary>
    internal async Task ResetBarWeightPicker()
    {
        // Remember the original selection.
        var selectedBarWeight = BarWeight;

        // Reset to force an update when the property is set again.
        BarWeight = 0;

        // Repopulate the picker options.
        var bars = await _barRepo.GetSome(enabled: true, ascending: true);
        BarWeights = bars.Select(b => b.Weight).ToList();

        // Select the previously selected value, if available.
        if (BarWeights.Contains(selectedBarWeight))
        {
            BarWeight = selectedBarWeight;
        }
        // Select the default value, if available.
        else if (BarWeights.Contains(BarRepository.DEFAULT_WEIGHT))
        {
            BarWeight = BarRepository.DEFAULT_WEIGHT;
        }
        // Otherwise, select the first value.
        else
        {
            BarWeight = BarWeights.FirstOrDefault();
        }
    }

    /// <summary>
    /// Set the user's preferred units, which may have changed on the settings page.
    /// </summary>
    internal void SetUnits()
    {
        var units = UnitsUtility.GetDefault().GetDescription();
        MaxWeightUnits = units;
        BarWeightUnits = units;
        StartingWeightUnits = units;
    }

    internal async Task Initialize()
    {
        // Set the units labels, which may have changed if the user went to the settings page.
        SetUnits();

        // Initialize the starting weight label.
        MachineTypeChanged();

        // Update the bar weight picker whenever this page appears, because the bar weights may have
        // changed on the Bars page.
        await ResetBarWeightPicker();
    }

    #endregion

    // ---------------------------------------------------------------------------------------------
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

    // ---------------------------------------------------------------------------------------------
    #region Command methods

    private async Task Calculate()
    {
        // Hide current results.
        ResultsVisible = false;

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
                throw new NoMatchingCaseException("Invalid exercise type.");
        }

        // Select 100% to start with.
        PercentSelected("100");
    }

    public void MachineTypeChanged()
    {
        StartingWeightLabel = MachineType == MachineType.Isolateral
            ? "Starting weight per side"
            : "Starting weight";
    }

    /// <summary>
    /// Command method for when a percent button is tapped.
    /// </summary>
    /// <param name="sPercent">
    /// The command parameter, which should be a string like "50", "60", etc.
    /// </param>
    public void PercentSelected(string sPercent)
    {
        SelectedPercent = int.TryParse(sPercent, out var percent) ? percent : 100;

        if (PlatesResultVisible)
        {
            SelectedPlatesResult = PlatesResults == null || PlatesResults.Count == 0
                ? null
                : PlatesResults.FirstOrDefault(r => r.Percent == SelectedPercent);
        }

        if (SingleWeightResultVisible)
        {
            SelectedSingleWeightResult =
                SingleWeightResults == null || SingleWeightResults.Count == 0
                    ? null
                    : SingleWeightResults.FirstOrDefault(r => r.Percent == SelectedPercent);
        }

        // Update percent button visual states.
        PercentButtonVisualState50 = SelectedPercent == 50 ? "Selected" : "Normal";
        PercentButtonVisualState60 = SelectedPercent == 60 ? "Selected" : "Normal";
        PercentButtonVisualState70 = SelectedPercent == 70 ? "Selected" : "Normal";
        PercentButtonVisualState80 = SelectedPercent == 80 ? "Selected" : "Normal";
        PercentButtonVisualState90 = SelectedPercent == 90 ? "Selected" : "Normal";
        PercentButtonVisualState100 = SelectedPercent == 100 ? "Selected" : "Normal";
    }

    #endregion Command methods

    // ---------------------------------------------------------------------------------------------
    #region Calculations

    private async Task DoBarbellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Calculate the results.
        var plates = await _plateRepo.GetSome(enabled: true, ascending: true);
        PlatesResults = PlateSolver.CalculateResults(MaxWeight!.Value, BarWeight, 2,
            "Plates each end", plates);

        // Display the results.
        ResultsExerciseType = ExerciseType.Barbell;
        PlatesResultVisible = true;
        SingleWeightResultVisible = false;
        ResultsVisible = true;
    }

    private async Task DoMachineCalculations()
    {
        if (!ValidateMaxWeight() || !ValidateStartingWeight())
        {
            return;
        }

        // Get the available plates.
        var plates = await _plateRepo.GetSome(enabled: true, ascending: true);

        // Determine the number of plate stacks and total starting weight from the machine type.
        var nStacks = MachineType == MachineType.Isolateral ? 2 : 1;
        var totalStartingWeight = StartingWeight!.Value * nStacks;
        var eachSideText = MachineType == MachineType.Isolateral ? "Plates each side" : "Plates";

        // Calculate the results.
        PlatesResults = PlateSolver.CalculateResults(MaxWeight!.Value, totalStartingWeight, nStacks,
            eachSideText, plates);

        // Display the results.
        ResultsExerciseType = ExerciseType.Machine;
        PlatesResultVisible = true;
        SingleWeightResultVisible = false;
        ResultsVisible = true;
    }

    private async Task DoDumbbellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Calculate the results.
        var dumbbells = await _dumbbellRepo.GetSome(enabled: true, ascending: true);
        SingleWeightResults = SingleWeightSolver.CalculateResults(MaxWeight!.Value, dumbbells);

        // Display the results.
        ResultsExerciseType = ExerciseType.Dumbbell;
        PlatesResultVisible = false;
        SingleWeightResultVisible = true;
        ResultsVisible = true;
    }

    private async Task DoKettlebellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Calculate the results.
        var kettlebells = await _kettlebellRepo.GetSome(enabled: true, ascending: true);
        SingleWeightResults = SingleWeightSolver.CalculateResults(MaxWeight!.Value, kettlebells);

        // Display the results.
        ResultsExerciseType = ExerciseType.Kettlebell;
        PlatesResultVisible = false;
        SingleWeightResultVisible = true;
        ResultsVisible = true;
    }

    #endregion Calculations
}
