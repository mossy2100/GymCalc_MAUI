using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using Galaxon.Core.Enums;
using Galaxon.Core.Exceptions;
using GymCalc.Solvers;
using GymCalc.Constants;
using GymCalc.Models;
using GymCalc.Data;
using GymCalc.Utilities;

namespace GymCalc.ViewModels;

public class CalculatorViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    #region Dependencies

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    #endregion Dependencies

    // ---------------------------------------------------------------------------------------------
    #region Bindable properties

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

    private string _maxWeightUnits;

    public string MaxWeightUnits
    {
        get => _maxWeightUnits;

        set => SetProperty(ref _maxWeightUnits, value);
    }

    private string _barWeightUnits;

    public string BarWeightUnits
    {
        get => _barWeightUnits;

        set => SetProperty(ref _barWeightUnits, value);
    }

    private string _startingWeightUnits;

    public string StartingWeightUnits
    {
        get => _startingWeightUnits;

        set => SetProperty(ref _startingWeightUnits, value);
    }

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

    private string _errorMessage;

    public string ErrorMessage
    {
        get => _errorMessage;

        set => SetProperty(ref _errorMessage, value);
    }

    #endregion Bindable properties

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// This is a non-bindable property set from the view.
    /// Could be made bindable, but as yet there's no need for that.
    /// </summary>
    public ExerciseType SelectedExerciseType { get; set; }

    // ---------------------------------------------------------------------------------------------
    #region Calculated properties

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

    #endregion

    // ---------------------------------------------------------------------------------------------
    #region Commands

    public ICommand CalculateCommand { get; init; }

    #endregion Commands

    // ---------------------------------------------------------------------------------------------
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public CalculatorViewModel(BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dumbbellRepo, KettlebellRepository kettlebellRepo)
    {
        // Keep references to the repositories.
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;

        // Create commands.
        CalculateCommand = new AsyncCommand(Calculate);
    }

    #endregion Constructors

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
                throw new NoMatchingCaseException("Invalid exercise type.");
        }
    }

    private async Task DoBarbellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Calculate and display the results.
        var plates = await _plateRepo.GetSome(enabled: true, ascending: true);
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
        var plates = await _plateRepo.GetSome(enabled: true, ascending: true);
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
        var dumbbells = await _dumbbellRepo.GetSome(enabled: true, ascending: true);
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
        var kettlebells = await _kettlebellRepo.GetSome(enabled: true, ascending: true);
        SingleWeightResults = SingleWeightSolver.CalculateResults(MaxWeight!.Value, kettlebells);
        SingleWeightResultsVisible = true;
    }

    #endregion Calculations

    // ---------------------------------------------------------------------------------------------
    #region Initialization stuff

    internal async Task InitializeDatabase()
    {
        var barTask = _barRepo.Initialize();
        var plateTask = _plateRepo.Initialize();
        var dumbbellTask = _dumbbellRepo.Initialize();
        var kettlebellTask = _kettlebellRepo.Initialize();
        await Task.WhenAll(new Task[] { barTask, plateTask, dumbbellTask, kettlebellTask });
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

    #endregion Initialization stuff
}
