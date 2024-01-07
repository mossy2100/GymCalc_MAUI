using AsyncAwaitBestPractices.MVVM;
using Galaxon.Core.Exceptions;
using GymCalc.Enums;
using GymCalc.Models;
using GymCalc.Repositories;
using GymCalc.Services;

namespace GymCalc.ViewModels;

public class CalculatorViewModel : BaseViewModel
{
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public CalculatorViewModel(BarRepository barRepo, PlateRepository plateRepo,
        BarbellRepository barbellRepo, DumbbellRepository dumbbellRepo,
        KettlebellRepository kettlebellRepo, CalculatorService calculatorService)
    {
        // Keep references to the repositories.
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _barbellRepo = barbellRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;
        _calculatorService = calculatorService;

        // Create commands.
        CalculateCommand = new AsyncCommand(Calculate);

        // Initial selected exercise type.
        SelectedExerciseType = EExerciseType.Barbell;
    }

    #endregion Constructors

    #region Dependencies

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly BarbellRepository _barbellRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    private readonly CalculatorService _calculatorService;

    #endregion Dependencies

    #region Bindable properties

    // ---------------------------------------------------------------------------------------------
    private string? _maxWeightText;

    public string? MaxWeightText
    {
        get => _maxWeightText;

        set => SetProperty(ref _maxWeightText, value);
    }

    // ---------------------------------------------------------------------------------------------
    private EBarbellType _barbellType = EBarbellType.PlateLoaded;

    public EBarbellType BarbellType
    {
        get => _barbellType;

        set => SetProperty(ref _barbellType, value);
    }

    // ---------------------------------------------------------------------------------------------
    private decimal _barWeight;

    public decimal BarWeight
    {
        get => _barWeight;

        set => SetProperty(ref _barWeight, value);
    }

    // ---------------------------------------------------------------------------------------------
    private List<decimal>? _barWeights;

    public List<decimal>? BarWeights
    {
        get => _barWeights;

        set => SetProperty(ref _barWeights, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _startingWeightText;

    public string? StartingWeightText
    {
        get => _startingWeightText;

        set => SetProperty(ref _startingWeightText, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _startingWeightLabel = "Starting weight";

    public string? StartingWeightLabel
    {
        get => _startingWeightLabel;

        set => SetProperty(ref _startingWeightLabel, value);
    }

    // ---------------------------------------------------------------------------------------------
    private EMovementType _movementType = EMovementType.Bilateral;

    public EMovementType MovementType
    {
        get => _movementType;

        set => SetProperty(ref _movementType, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _maxWeightUnits;

    public string? MaxWeightUnits
    {
        get => _maxWeightUnits;

        set => SetProperty(ref _maxWeightUnits, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _barWeightUnits;

    public string? BarWeightUnits
    {
        get => _barWeightUnits;

        set => SetProperty(ref _barWeightUnits, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _startingWeightUnits;

    public string? StartingWeightUnits
    {
        get => _startingWeightUnits;

        set => SetProperty(ref _startingWeightUnits, value);
    }

    // ---------------------------------------------------------------------------------------------
    private EExerciseType _selectedExerciseType;

    internal EExerciseType SelectedExerciseType
    {
        get => _selectedExerciseType;

        set => SetProperty(ref _selectedExerciseType, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _errorMessage;

    public string? ErrorMessage
    {
        get => _errorMessage;

        set => SetProperty(ref _errorMessage, value);
    }

    #endregion Bindable properties

    #region Calculated properties

    /// <summary>
    /// Determine the maximum weight from the entry control.
    /// Treat blank as equal to 0.
    /// Any other non-numeric value will return null.
    /// </summary>
    private decimal? MaxWeight =>
        string.IsNullOrEmpty(MaxWeightText)
            ? 0
            : decimal.TryParse(MaxWeightText, out decimal maxWeight)
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
            : decimal.TryParse(StartingWeightText, out decimal startingWeight)
                ? startingWeight
                : null;

    #endregion

    #region Commands

    public ICommand CalculateCommand { get; init; }

    #endregion Commands

    #region Initialization stuff

    internal async Task InitializeDatabase()
    {
        Task barTask = _barRepo.Initialize();
        Task plateTask = _plateRepo.Initialize();
        Task barbellTask = _barbellRepo.Initialize();
        Task dumbbellTask = _dumbbellRepo.Initialize();
        Task kettlebellTask = _kettlebellRepo.Initialize();
        await Task.WhenAll(barTask, plateTask, barbellTask, dumbbellTask, kettlebellTask);
    }

    /// <summary>
    /// Reset the bar weight picker items.
    /// </summary>
    private async Task ResetBarWeightPicker()
    {
        // Remember the original selection.
        decimal selectedBarWeight = BarWeight;

        // Reset to force an update when the property is set again.
        BarWeight = 0;

        // Repopulate the picker options.
        List<Bar> bars = await _barRepo.LoadSome();
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
    private void SetUnits()
    {
        string sUnits = UnitsService.GetDefaultUnitsSymbol();
        MaxWeightUnits = sUnits;
        BarWeightUnits = sUnits;
        StartingWeightUnits = sUnits;
    }

    internal async Task Initialize()
    {
        // Set the units labels, which may have changed if the user went to the settings page.
        SetUnits();

        // Update the bar weight picker whenever this page appears, because the bar weights may have
        // changed on the Bars page.
        await ResetBarWeightPicker();
    }

    #endregion

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

    #region Command methods

    private async Task Calculate()
    {
        bool proceed = ValidateMaxWeight();
        if (!proceed)
        {
            return;
        }

        // Do the calculations based on the selected exercise type.
        switch (SelectedExerciseType)
        {
            case EExerciseType.Barbell:
                await _calculatorService.DoBarbellCalculations(BarbellType, MaxWeight!.Value,
                    BarWeight);
                break;

            case EExerciseType.Machine:
                if (proceed && ValidateStartingWeight())
                {
                    await _calculatorService.DoMachineCalculations(MovementType, MaxWeight!.Value,
                        StartingWeight!.Value);
                }
                else
                {
                    return;
                }
                break;

            case EExerciseType.Dumbbell:
                await _calculatorService.DoDumbbellCalculations(MaxWeight!.Value);
                break;

            case EExerciseType.Kettlebell:
                await _calculatorService.DoKettlebellCalculations(MaxWeight!.Value);
                break;

            default:
                throw new MatchNotFoundException("Invalid exercise type.");
        }

        // Go to the results page.
        await Shell.Current.GoToAsync("results");
    }

    #endregion Command methods
}
