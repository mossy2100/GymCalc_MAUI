using Galaxon.Core.Types;
using Galaxon.Maui.Utilities;
using GymCalc.Controls;
using GymCalc.Enums;
using GymCalc.Services;
using GymCalc.Shared;
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

        // Update the calculator settings.
        UpdateCalculatorSettings();

        // Display the right type of result.
        _model.PlatesResultVisible = _calculatorService.ResultType == EResultType.Plates;
        _model.SingleWeightResultVisible =
            _calculatorService.ResultType == EResultType.SingleWeight;

        // Select the 100% result to start with.
        _model.PercentSelected(100);
    }

    private void UpdateCalculatorSettings()
    {
        MauiUtility.ClearGrid(CalculatorSettingsGrid, false, true);
        Dictionary<string, string> rows = new ();
        string sUnits = UnitsUtility.GetDefault().GetDescription();
        if (_calculatorService.ExerciseType != null)
        {
            rows.Add("Exercise type", _calculatorService.ExerciseType.GetDescription());
        }
        if (_calculatorService.MaxWeight != null)
        {
            rows.Add("Maximum weight", $"{_calculatorService.MaxWeight} {sUnits}");
        }
        if (_calculatorService.BarbellType != null)
        {
            rows.Add("Barbell type", _calculatorService.BarbellType.GetDescription());
        }
        if (_calculatorService.BarWeight != null)
        {
            rows.Add("Bar weight", $"{_calculatorService.BarWeight} {sUnits}");
        }
        if (_calculatorService.MachineType != null)
        {
            rows.Add("Machine type", _calculatorService.MachineType.GetDescription());
        }
        if (_calculatorService.StartingWeight != null)
        {
            rows.Add("Starting weight", $"{_calculatorService.StartingWeight} {sUnits}");
        }
        var i = 0;
        var fontSize = 14;
        foreach (KeyValuePair<string, string> row in rows)
        {
            CalculatorSettingsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            CalculatorSettingsGrid.Add(new BoldLabel
            {
                Text = row.Key,
                FontSize = fontSize
            }, 0, i);
            CalculatorSettingsGrid.Add(new Label
            {
                Text = row.Value,
                FontSize = fontSize
            }, 1, i);
            i++;
        }
    }

    #endregion Events
}
