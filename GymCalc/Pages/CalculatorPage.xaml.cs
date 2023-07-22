using System.Globalization;
using Microsoft.Maui.Controls.Shapes;
using GymCalc.Calculations;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Graphics;
using GymCalc.Utilities;

namespace GymCalc.Pages;

public partial class CalculatorPage : ContentPage
{
    private static double _maxWeight;

    private static double _barWeight;

    private bool _databaseInitialized;

    private static ExerciseType _selectedExerciseType = ExerciseType.Barbell;

    private Dictionary<double, Plate> _availPlates;

    private Dictionary<double, List<double>> _barbellResults;

    private Dictionary<double, double> _dumbbellResults;

    public CalculatorPage()
    {
        InitializeComponent();
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
        CalculatorLayout.Loaded += CalculatorLayoutLoaded;
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
        UpdateExerciseType(_selectedExerciseType);

        // Update the bar weight picker whenever this page appears, because the bar weights may have
        // changed on the Bars page.
        await ResetBarWeightPicker();
    }

    private void CalculatorLayoutLoaded(object sender, EventArgs eventArgs)
    {
        UpdateLayoutOrientation(true);
    }

    private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        UpdateLayoutOrientation();
    }

    private void UpdateLayoutOrientation(bool forceRedraw = false)
    {
        // Get the device orientation.
        var newOrientation =
            DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape
                ? StackOrientation.Horizontal
                : StackOrientation.Vertical;

        // Skip the redraw if we don't need to do it.
        if (!forceRedraw && newOrientation == CalculatorLayout.Orientation)
        {
            return;
        }

        // If different, update the layout.
        CalculatorLayout.Orientation = newOrientation;

        // Redraws the page, which updates the CalculatorLayout orientation and width.
        // This is only needed for Android, not iOS, but it doesn't do any harm.
        InvalidateMeasure();

        // Update the button widths.
        SetExerciseTypeButtonWidths();

        // Re-render the results for the altered width.
        switch (_selectedExerciseType)
        {
            case ExerciseType.Barbell:
                DisplayBarbellResults();
                break;

            case ExerciseType.Dumbbell:
                DisplayDumbbellResults();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private double GetAvailWidth()
    {
        return (CalculatorLayout.Width / App.GetNumColumns()) - (2 * App.Spacing);
    }

    private void SetExerciseTypeButtonWidths()
    {
        // Reset the button widths to some small value so they don't push the form out wider than
        // the visible area.
        BarbellButton.WidthRequest = 100;
        DumbbellButton.WidthRequest = 100;

        // Calculate the button width.
        const int nButtons = 2;
        var buttonWidth = (GetAvailWidth() - ((nButtons - 1) * App.Spacing)) / nButtons;

        // Set the button widths.
        BarbellButton.WidthRequest = buttonWidth;
        DumbbellButton.WidthRequest = buttonWidth;
    }

    private void UpdateExerciseType(ExerciseType exerciseType)
    {
        _selectedExerciseType = exerciseType;

        switch (exerciseType)
        {
            case ExerciseType.Barbell:
                // Update the button visual states.
                VisualStateManager.GoToState(BarbellButton, "Selected");
                VisualStateManager.GoToState(DumbbellButton, "Normal");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum total weight (kg)";

                // Show the bar weight fields.
                BarWeightLabel.IsVisible = true;
                BarWeightPickerFrame.IsVisible = true;

                // Show the bar weight row and adjust the spacing.
                CalculatorFormGrid.RowDefinitions[1].Height = GridLength.Auto;
                CalculatorFormGrid.RowSpacing = App.Spacing;
                break;

            case ExerciseType.Dumbbell:
                // Update the button visual states.
                VisualStateManager.GoToState(DumbbellButton, "Selected");
                VisualStateManager.GoToState(BarbellButton, "Normal");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Max. weight per dumbbell (kg)";

                // Hide the bar weight fields.
                BarWeightLabel.IsVisible = false;
                BarWeightPickerFrame.IsVisible = false;

                // Hide the bar weight row and adjust the spacing.
                CalculatorFormGrid.RowDefinitions[1].Height = new GridLength(0);
                CalculatorFormGrid.RowSpacing = 0;
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
        switch (_selectedExerciseType)
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

                // Get the available plates.
                var plates = await PlateRepository.GetAll(true);
                _availPlates = plates.ToDictionary(p => p.Weight, p => p);

                // Calculate and display the results.
                var barbellSolver = new BarbellSolver();
                _barbellResults =
                    barbellSolver.CalculateResults(_maxWeight, _barWeight, _availPlates);
                DisplayBarbellResults();
                break;

            case ExerciseType.Dumbbell:
                // Get the max dumbbell weight from the calculator field.
                if (!double.TryParse(MaxWeightEntry.Text, out _maxWeight))
                {
                    ErrorMessage.Text = "Please enter the maximum weight per dumbbell.";
                    return;
                }

                // Calculate and display the results.
                _dumbbellResults = await DumbbellCalculation.CalculateResults(_maxWeight);
                DisplayDumbbellResults();
                break;
        }
    }

    private void DisplayBarbellResults()
    {
        // Clear the error message.
        ErrorMessage.Text = "";

        // Clear the results.
        MauiUtilities.ClearStack(CalculatorResults);

        // Check if there aren't any results to render.
        if (_barbellResults == null)
        {
            return;
        }

        // Prepare the styles.
        var percentStyle = MauiUtilities.LookupStyle("ResultsTablePercent");
        var headerStyle = MauiUtilities.LookupStyle("ResultsTableHeader");
        var weightStyle = MauiUtilities.LookupStyle("ResultsTableWeight");
        var focusWeightStyle = MauiUtilities.LookupStyle("ResultsTableFocusWeight");

        // Get the available width in device-independent pixels.
        var availWidth = GetAvailWidth();

        var maxPlateWeight = _availPlates.Keys.Max();

        foreach (var (percent, platesResult) in _barbellResults)
        {
            // Horizontal rule.
            CalculatorResults.Add(GetHorizontalRule(availWidth));

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
                Padding = new Thickness(0, App.DoubleSpacing, 0, 0),
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

            CalculatorResults.Add(textGrid);

            // Construct the plates grid and add it to the layout.
            var platesGrid = new Grid
            {
                Padding = new Thickness(0, App.Spacing, 0, App.DoubleSpacing)
            };
            var j = 0;
            platesResult.Sort();
            foreach (var plateWeight in platesResult)
            {
                platesGrid.RowDefinitions.Add(new RowDefinition());
                PlatesPage.AddPlateToGrid(_availPlates[plateWeight], platesGrid, 0, j,
                    maxPlateWeight);
                j++;
            }
            CalculatorResults.Add(platesGrid);
        }

        // Horizontal rule.
        CalculatorResults.Add(GetHorizontalRule(availWidth));
    }

    private void DisplayDumbbellResults()
    {
        // Clear the error message.
        ErrorMessage.Text = "";

        // Clear the results.
        MauiUtilities.ClearStack(CalculatorResults);

        // Check if there aren't any results to render.
        if (_dumbbellResults == null)
        {
            return;
        }

        // Prepare the styles.
        var percentStyle = MauiUtilities.LookupStyle("ResultsTablePercent");
        var headerStyle = MauiUtilities.LookupStyle("ResultsTableHeader");
        var weightStyle = MauiUtilities.LookupStyle("ResultsTableWeight");
        var barLabelStyle = MauiUtilities.LookupStyle("BarLabelStyle");

        // Get the available width in device-independent pixels.
        var availWidth = GetAvailWidth();

        // Dumbbell graphic dimensions.
        const int dumbbellHeight = 50;
        const int dumbbellWidth = 100;

        foreach (var (percent, closestWeight) in _dumbbellResults)
        {
            // Horizontal rule.
            CalculatorResults.Add(GetHorizontalRule(availWidth));

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
                Padding = new Thickness(0, App.DoubleSpacing, 0, App.DoubleSpacing),
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

            CalculatorResults.Add(textGrid);
        }

        // Horizontal rule.
        CalculatorResults.Add(GetHorizontalRule(availWidth));
    }

    private static Rectangle GetHorizontalRule(double width)
    {
        return new Rectangle
        {
            BackgroundColor = Colors.Grey,
            WidthRequest = width,
            HeightRequest = 1,
        };
    }
}
