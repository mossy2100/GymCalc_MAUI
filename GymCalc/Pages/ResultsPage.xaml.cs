using GymCalc.Constants;
using GymCalc.Services;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class ResultsPage : ContentPage
{
    #region Fields

    private readonly ResultsViewModel _model;

    private readonly CalculatorService _calculatorService;

    #endregion Fields

    #region Constructor

    public ResultsPage(ResultsViewModel model, CalculatorService calculatorService)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
        _calculatorService = calculatorService;
    }

    #endregion Constructor

    #region Events

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Display the right type of result.
        _model.PlatesResultVisible = _calculatorService.SelectedResultType == ResultType.Plates;
        _model.SingleWeightResultVisible =
            _calculatorService.SelectedResultType == ResultType.SingleWeight;

        // Select the 100% result to start with.
        _model.PercentSelected(100);
    }

    #endregion Events
}
