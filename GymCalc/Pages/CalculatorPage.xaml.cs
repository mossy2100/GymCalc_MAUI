using System.Globalization;
using Microsoft.Maui.Controls.Shapes;
using GymCalc.Calculations;
using GymCalc.Data;
using GymCalc.Data.Repositories;
using GymCalc.Graphics;
using GymCalc.Utilities;

namespace GymCalc.Pages;

public partial class CalculatorPage : ContentPage
{
    private static double _maxWeight;

    private static double _barWeight;

    private bool _databaseInitialized;

    public CalculatorPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        // Initialize database on first page load.
        if (!_databaseInitialized)
        {
            await Database.Initialize();
            _databaseInitialized = true;
        }

        // Initialise the exercise type buttons.
        SetExerciseTypeButtonWidths();
        UpdateExerciseType(App.SelectedExerciseType);

        // Update the bar weight picker whenever this page appears, because the bar weights may have
        // changed on the Bars page.
        await ResetBarWeightPicker();
    }

    private void SetExerciseTypeButtonWidths()
    {
        var deviceWidth =
            DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        const int nButtons = 2;
        var buttonWidth = (deviceWidth - 30) / nButtons;
        BarbellButton.WidthRequest = buttonWidth;
        DumbbellButton.WidthRequest = buttonWidth;
    }

    private void UpdateExerciseType(ExerciseType exerciseType)
    {
        App.SelectedExerciseType = exerciseType;

        switch (exerciseType)
        {
            case ExerciseType.Barbell:
                VisualStateManager.GoToState(BarbellButton, "Selected");
                VisualStateManager.GoToState(DumbbellButton, "Normal");
                MaxWeightLabel.Text = "Maximum total weight (kg)";
                CalculatorFormGrid.RowDefinitions[1].Height = GridLength.Auto;
                CalculatorFormGrid.Padding = new Thickness(0, 0, 0, 10);
                BarWeightLabel.IsVisible = true;
                BarWeightPickerFrame.IsVisible = true;
                break;

            case ExerciseType.Dumbbell:
                VisualStateManager.GoToState(DumbbellButton, "Selected");
                VisualStateManager.GoToState(BarbellButton, "Normal");
                MaxWeightLabel.Text = "Maximum weight per dumbbell (kg)";
                BarWeightLabel.IsVisible = false;
                BarWeightPickerFrame.IsVisible = false;
                CalculatorFormGrid.RowDefinitions[1].Height = new GridLength(0);
                CalculatorFormGrid.Padding = new Thickness(0, 0, 0, 0);
                break;
        }
    }

    private void OnBarbellButtonClicked(object sender, EventArgs e)
    {
        UpdateExerciseType(ExerciseType.Barbell);
    }

    private void OnDumbbellButtonClicked(object sender, EventArgs e)
    {
        UpdateExerciseType(ExerciseType.Dumbbell);
    }

    /// <summary>
    /// Reset the bar weight picker items.
    /// </summary>
    private async Task ResetBarWeightPicker()
    {
        // Get the current selected value.
        var initialSelectedIndex = BarWeightPicker.SelectedIndex;
        var initialSelectedValue = initialSelectedIndex != -1
            ? BarWeightPicker.Items[initialSelectedIndex]
            : null;

        // Reset the picker items.
        BarWeightPicker.Items.Clear();
        BarWeightPicker.SelectedIndex = -1;

        // Get the available bar weights ordered by weight.
        var bars = await BarRepository.GetAll(true);

        // Initialise the items in the bar weight picker.
        var i = 0;
        var valueSelected = false;
        foreach (var bar in bars)
        {
            // Add the picker item.
            var weightString = bar.Weight.ToString(CultureInfo.InvariantCulture);
            BarWeightPicker.Items.Add(weightString);

            // Try to select the same weight that was selected before.
            if (!valueSelected && weightString == initialSelectedValue)
            {
                BarWeightPicker.SelectedIndex = i;
                valueSelected = true;
            }

            i++;
        }

        // If the original selected bar weight is no longer present, try to select the default.
        if (!valueSelected)
        {
            var weightString = BarRepository.DEFAULT_WEIGHT.ToString(CultureInfo.InvariantCulture);
            for (i = 0; i < BarWeightPicker.Items.Count; i++)
            {
                // Default selection.
                if (BarWeightPicker.Items[i] == weightString)
                {
                    BarWeightPicker.SelectedIndex = i;
                    valueSelected = true;
                    break;
                }
            }
        }

        // If no bar weight has been selected yet, select the first one.
        if (!valueSelected)
        {
            BarWeightPicker.SelectedIndex = 0;
        }
    }

    private async void OnCalculateButtonClicked(object sender, EventArgs e)
    {
        switch (App.SelectedExerciseType)
        {
            case ExerciseType.Barbell:
                // Get the weights from the calculator fields and validate them.
                _barWeight = double.Parse(BarWeightPicker.Items[BarWeightPicker.SelectedIndex]);
                if (!double.TryParse(MaxWeightEntry.Text, out _maxWeight))
                {
                    ErrorMessage.Text = "Please enter the maximum total weight, including the bar.";
                    return;
                }
                if (_maxWeight < _barWeight)
                {
                    ErrorMessage.Text =
                        "Make sure the maximum weight is greater than or equal to the bar weight.";
                    return;
                }

                // Calculate and display the results.
                var barbellSolver = new BarbellSolver();
                var barbellResult = await barbellSolver.CalculateResults(_maxWeight, _barWeight);
                await DisplayBarbellResults(barbellResult);
                break;

            case ExerciseType.Dumbbell:
                // Get the max dumbbell weight from the calculator field.
                if (!double.TryParse(MaxWeightEntry.Text, out _maxWeight))
                {
                    ErrorMessage.Text = "Please enter the maximum weight per dumbbell.";
                    return;
                }

                // Calculate and display the results.
                var dumbbellResult = await DumbbellCalculation.CalculateResults(_maxWeight);
                DisplayDumbbellResults(dumbbellResult);
                break;
        }
    }

    private async Task DisplayBarbellResults(Dictionary<double, List<double>> results)
    {
        // Clear the error message.
        ErrorMessage.Text = "";

        // Clear the old results.
        while (CalculatorLayout.Children[CalculatorLayout.Count - 1] != ErrorMessage)
        {
            CalculatorLayout.RemoveAt(CalculatorLayout.Count - 1);
        }

        // Prepare the styles.
        var percentStyle = MauiUtilities.LookupStyle("ResultsTablePercent");
        var headerStyle = MauiUtilities.LookupStyle("ResultsTableHeader");
        var weightStyle = MauiUtilities.LookupStyle("ResultsTableWeight");
        var focusWeightStyle = MauiUtilities.LookupStyle("ResultsTableFocusWeight");

        // Get the device width in device-independent pixels.
        var deviceWidth =
            DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

        // Get the available plates as a dictionary mapping weights to Plate objects.
        var plates = await PlateRepository.GetAll(true);
        var plateLookup = plates.ToDictionary(p => p.Weight, p => p);

        foreach (var (percent, platesResult) in results)
        {
            // Horizontal rule.
            var horizontalLine = new Rectangle
            {
                BackgroundColor = Colors.Grey,
                WidthRequest = deviceWidth - 20,
                HeightRequest = 1,
            };
            CalculatorLayout.Add(horizontalLine);

            // Table of results for this percentage.
            var textGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection(
                    new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(2, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(2, GridUnitType.Star))
                ),
                RowDefinitions = new RowDefinitionCollection(
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto)
                ),
                ColumnSpacing = 15,
                RowSpacing = 15,
                Padding = new Thickness(0, 20, 0, 0),
            };

            ///////////
            // Row 0.

            // Percentage heading.
            var percentLabel = new Label
            {
                FormattedText = TextUtility.StyleText($"{percent}%", percentStyle),
            };
            textGrid.Add(percentLabel, 0);

            // Ideal heading.
            var idealHeading = new Label
            {
                FormattedText = TextUtility.StyleText("Ideal", headerStyle),
                VerticalTextAlignment = TextAlignment.End,
            };
            textGrid.Add(idealHeading, 1);

            // Closest heading.
            var closestHeading = new Label
            {
                FormattedText = TextUtility.StyleText("Closest", headerStyle),
                VerticalTextAlignment = TextAlignment.End,
            };
            textGrid.Add(closestHeading, 2);

            ///////////
            // Row 1.

            // Total including bar heading.
            var totalHeading = new Label
            {
                FormattedText = TextUtility.StyleText("Total including bar", headerStyle),
            };
            textGrid.Add(totalHeading, 0, 1);

            // Ideal total weight.
            var idealTotal = _maxWeight * percent / 100.0;
            var idealPlates = (idealTotal - _barWeight) / 2;
            var idealTotalValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{idealTotal:F2} kg", weightStyle),
            };
            textGrid.Add(idealTotalValue, 1, 1);

            // Closest total weight.
            var closestPlates = platesResult.Sum();
            var closestTotal = 2 * closestPlates + _barWeight;
            var closestTotalValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{closestTotal:F2} kg", weightStyle),
            };
            textGrid.Add(closestTotalValue, 2, 1);

            ///////////
            // Row 2.

            // Plates per end heading.
            var platesPerEndHeading = new Label
            {
                FormattedText = TextUtility.StyleText("Plates per end", headerStyle),
            };
            textGrid.Add(platesPerEndHeading, 0, 2);

            // Ideal plates weight.
            var idealPlatesValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{idealPlates:F2} kg", weightStyle),
            };
            textGrid.Add(idealPlatesValue, 1, 2);

            // Closest plates weight.
            var closestPlatesValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{closestPlates:F2} kg", focusWeightStyle),
            };
            textGrid.Add(closestPlatesValue, 2, 2);

            CalculatorLayout.Add(textGrid);

            // Construct the plates grid and add it to the layout.
            var platesGrid = new Grid { Padding = new Thickness(0, 10, 0, 20) };
            var j = 0;
            platesResult.Sort();
            foreach (var plateWeight in platesResult)
            {
                platesGrid.RowDefinitions.Add(new RowDefinition());
                PlatesPage.AddPlateToGrid(plateLookup[plateWeight], platesGrid, 0, j);
                j++;
            }
            CalculatorLayout.Add(platesGrid);
        }
    }

    private void DisplayDumbbellResults(Dictionary<double, double> results)
    {
        // Clear the error message.
        ErrorMessage.Text = "";

        // Clear the old results.
        while (CalculatorLayout.Count > 4)
        {
            CalculatorLayout.RemoveAt(CalculatorLayout.Count - 1);
        }

        // Prepare the styles.
        var percentStyle = MauiUtilities.LookupStyle("ResultsTablePercent");
        var headerStyle = MauiUtilities.LookupStyle("ResultsTableHeader");
        var weightStyle = MauiUtilities.LookupStyle("ResultsTableWeight");
        var barLabelStyle = MauiUtilities.LookupStyle("BarLabelStyle");

        // Get the device width in device-independent pixels.
        var deviceWidth =
            DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

        // Dumbbell graphic dimensions.
        const int dumbbellHeight = 50;
        const int dumbbellWidth = 100;

        foreach (var (percent, closestWeight) in results)
        {
            // Horizontal rule.
            var horizontalLine = new Rectangle
            {
                BackgroundColor = Colors.Grey,
                WidthRequest = deviceWidth - 20,
                HeightRequest = 1,
            };
            CalculatorLayout.Add(horizontalLine);

            // Table of results for this percentage.
            var textGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection(
                    new ColumnDefinition(new GridLength(2, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(3, GridUnitType.Star))
                ),
                RowDefinitions = new RowDefinitionCollection(
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto)
                ),
                ColumnSpacing = 15,
                RowSpacing = 15,
                Padding = new Thickness(0, 20, 0, 20),
            };

            ///////////
            // Row 0.

            // Percentage heading.
            var percentLabel = new Label
            {
                FormattedText = TextUtility.StyleText($"{percent}%", percentStyle),
            };
            textGrid.Add(percentLabel, 0, 0);

            // Ideal heading.
            var idealHeading = new Label
            {
                FormattedText = TextUtility.StyleText("Ideal", headerStyle),
                VerticalTextAlignment = TextAlignment.End,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            textGrid.Add(idealHeading, 1, 0);

            // Closest heading.
            var closestHeading = new Label
            {
                FormattedText = TextUtility.StyleText("Closest", headerStyle),
                VerticalTextAlignment = TextAlignment.End,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            textGrid.Add(closestHeading, 2, 0);

            ///////////
            // Row 1.

            // Ideal dumbbell weight.
            var idealWeight = _maxWeight * percent / 100.0;
            var idealValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{idealWeight:F2} kg", weightStyle),
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            textGrid.Add(idealValue, 1, 1);

            // Closest dumbbell.

            // Dumbbell background.
            var dumbbellGraphic = new GraphicsView
            {
                Drawable = new DumbbellGraphic(),
                HeightRequest = dumbbellHeight,
                WidthRequest = dumbbellWidth,
            };
            textGrid.Add(dumbbellGraphic, 2, 1);

            // Dumbbell label.
            var closestValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{closestWeight}", barLabelStyle),
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            textGrid.Add(closestValue, 2, 1);

            CalculatorLayout.Add(textGrid);
        }
    }
}
