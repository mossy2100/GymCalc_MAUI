using Galaxon.Core.Types;
using Galaxon.Maui.Utilities;
using GymCalc.Controls;
using GymCalc.Enums;
using GymCalc.Graphics;
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

        // Update the calculator settings.
        UpdateCalculatorSettings();

        // Display the right type of result.
        _model.PlatesResultVisible = _calculatorService.ResultType == EResultType.Plates;
        _model.SingleWeightResultVisible =
            _calculatorService.ResultType == EResultType.SingleWeight;

        // Select the 100% result to start with.
        _model.PercentSelected(100);
    }

    #endregion Events

    #region UI

    private void UpdateCalculatorSettings()
    {
        MauiUtility.ClearGrid(CalculatorSettingsGrid, false, true);
        Dictionary<string, string> rows = new ();
        string sUnits = UnitsService.GetDefaultUnitsSymbol();
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
        if (_calculatorService.MovementType != null)
        {
            rows.Add("Machine type", _calculatorService.MovementType.GetDescription());
        }
        if (_calculatorService.StartingWeight != null)
        {
            string startingWeightText = _calculatorService.MovementType == EMovementType.Isolateral
                ? "Starting weight per side"
                : "Starting weight";
            rows.Add(startingWeightText, $"{_calculatorService.StartingWeight} {sUnits}");
        }
        var i = 0;
        foreach (KeyValuePair<string, string> row in rows)
        {
            CalculatorSettingsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            CalculatorSettingsGrid.Add(new BoldLabel
            {
                Text = row.Key,
                FontSize = TextSize.SMALL,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, i);
            CalculatorSettingsGrid.Add(new Label
            {
                Text = row.Value,
                FontSize = TextSize.SMALL,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 1, i);
            i++;
        }
    }

    #endregion UI
}
