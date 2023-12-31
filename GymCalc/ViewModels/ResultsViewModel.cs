using GymCalc.Drawables;
using GymCalc.Enums;
using GymCalc.Models;
using GymCalc.Services;

namespace GymCalc.ViewModels;

public class ResultsViewModel : BaseViewModel
{
    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    public ResultsViewModel(CalculatorService calculatorService)
    {
        // Keep references to the repositories.
        CalculatorService = calculatorService;

        // Create commands.
        PercentSelectedCommand = new Command<string>(PercentSelected);

        // Select 100% to start with.
        PercentSelected(100);
    }

    #endregion Constructor

    #region Dependencies

    public CalculatorService CalculatorService { get; init; }

    #endregion Dependencies

    #region Bindable properties

    // ---------------------------------------------------------------------------------------------
    private int _selectedPercent;

    public int SelectedPercent
    {
        get => _selectedPercent;

        set => SetProperty(ref _selectedPercent, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _percentButtonVisualState50;

    public string? PercentButtonVisualState50
    {
        get => _percentButtonVisualState50;

        set => SetProperty(ref _percentButtonVisualState50, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _percentButtonVisualState60;

    public string? PercentButtonVisualState60
    {
        get => _percentButtonVisualState60;

        set => SetProperty(ref _percentButtonVisualState60, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _percentButtonVisualState70;

    public string? PercentButtonVisualState70
    {
        get => _percentButtonVisualState70;

        set => SetProperty(ref _percentButtonVisualState70, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _percentButtonVisualState80;

    public string? PercentButtonVisualState80
    {
        get => _percentButtonVisualState80;

        set => SetProperty(ref _percentButtonVisualState80, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _percentButtonVisualState90;

    public string? PercentButtonVisualState90
    {
        get => _percentButtonVisualState90;

        set => SetProperty(ref _percentButtonVisualState90, value);
    }

    // ---------------------------------------------------------------------------------------------
    private string? _percentButtonVisualState100;

    public string? PercentButtonVisualState100
    {
        get => _percentButtonVisualState100;

        set => SetProperty(ref _percentButtonVisualState100, value);
    }

    // ---------------------------------------------------------------------------------------------
    private PlatesResult? _selectedPlatesResult;

    public PlatesResult? SelectedPlatesResult
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
    private SingleWeightResult? _selectedSingleWeightResult;

    public SingleWeightResult? SelectedSingleWeightResult
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
    private BaseDrawable? _selectedResultDrawable;

    public BaseDrawable? SelectedResultDrawable
    {
        get => _selectedResultDrawable;

        set => SetProperty(ref _selectedResultDrawable, value);
    }

    #endregion Bindable properties

    #region Commands

    // ---------------------------------------------------------------------------------------------
    public ICommand PercentSelectedCommand { get; init; }

    /// <summary>
    /// Command method for when a percent button is tapped.
    /// </summary>
    /// <param name="sPercent">
    /// The command parameter, which should be a string like "50", "60", etc.
    /// </param>
    public void PercentSelected(string sPercent)
    {
        // Try to convert the string to an integer and check it's valid. If the string can't be
        // converted to a valid percent, don't throw an exception; just default to 100.
        int[] validPercentages = [50, 60, 70, 80, 90, 100];
        if (!int.TryParse(sPercent, out int percent) || !validPercentages.Contains(percent))
        {
            percent = 100;
        }

        // Call the integer version of the method.
        PercentSelected(percent);
    }

    #endregion Commands

    #region Methods

    /// <summary>
    /// Get the visual state for a percent button.
    /// </summary>
    /// <param name="pc">The percentage value of the percent button.</param>
    /// <returns>The button's visual state.</returns>
    private string GetPercentButtonVisualState(int pc)
    {
        return SelectedPercent == pc ? "Selected" : "Normal";
    }

    /// <summary>
    /// Method for when a percent button is tapped.
    /// </summary>
    /// <param name="percent">
    /// The selected percent (50, 60, 70, 80, 90, or 100).
    /// </param>
    internal void PercentSelected(int percent)
    {
        // Remember the selected percent.
        SelectedPercent = percent;

        // Display the matching result.
        switch (CalculatorService.ResultType)
        {
            case EResultType.Plates:
                SelectedPlatesResult =
                    CalculatorService.PlatesResults?.FirstOrDefault(r => r.Percent == SelectedPercent);
                SelectedResultDrawable = SelectedPlatesResult?.Drawable;
                break;

            case EResultType.SingleWeight:
                SelectedSingleWeightResult =
                    CalculatorService.SingleWeightResults?.FirstOrDefault(r =>
                        r.Percent == SelectedPercent);
                SelectedResultDrawable = SelectedSingleWeightResult?.Drawable;
                break;

            default:
                SelectedPlatesResult = null;
                SelectedSingleWeightResult = null;
                break;
        }

        // Update all the percent button visual states.
        PercentButtonVisualState50 = GetPercentButtonVisualState(50);
        PercentButtonVisualState60 = GetPercentButtonVisualState(60);
        PercentButtonVisualState70 = GetPercentButtonVisualState(70);
        PercentButtonVisualState80 = GetPercentButtonVisualState(80);
        PercentButtonVisualState90 = GetPercentButtonVisualState(90);
        PercentButtonVisualState100 = GetPercentButtonVisualState(100);
    }

    #endregion Methods
}
