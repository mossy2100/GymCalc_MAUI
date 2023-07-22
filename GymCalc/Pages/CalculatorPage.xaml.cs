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
    private bool _databaseInitialized;

    private static ExerciseType _selectedExerciseType = ExerciseType.Barbell;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Values extracted from the form.
    private double _maxWeight;

    private double _barWeight;

    private double _startingWeight;

    private bool _oneSideOnly;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Lookup tables with cached database objects (weights).
    private Dictionary<double, Bar> _barLookup;

    private Dictionary<double, Plate> _plateLookup;

    private Dictionary<double, Dumbbell> _dumbbellLookup;

    private Dictionary<double, Kettlebell> _kettlebellLookup;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Results.
    private Dictionary<double, List<double>> _barbellResults;

    private Dictionary<double, double> _dumbbellResults;

    private Dictionary<double, List<double>> _machineResults;

    private Dictionary<double, double> _kettlebellResults;

    /// <summary>
    /// Constructor.
    /// </summary>
    public CalculatorPage()
    {
        InitializeComponent();
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
        CalculatorLayout.Loaded += OnCalculatorLayoutLoaded;
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

        // Invalidate collections because they may have changed on one of the other pages.
        _barLookup = null;
        _plateLookup = null;
        _dumbbellLookup = null;
        _kettlebellLookup = null;

        // Initialise the exercise type buttons.
        UpdateExerciseType(_selectedExerciseType);

        // Update the bar weight picker whenever this page appears, because the bar weights may have
        // changed on the Bars page.
        await ResetBarWeightPicker();
    }

    private void OnCalculatorLayoutLoaded(object sender, EventArgs eventArgs)
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

            case ExerciseType.Machine:
                DisplayMachineResults();
                break;

            case ExerciseType.Kettlebell:
                DisplayKettlebellResults();
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
        var width = (GetAvailWidth() - App.Spacing) / 2;
        BarbellButton.WidthRequest = width;
        DumbbellButton.WidthRequest = width;
        MachineButton.WidthRequest = width;
        KettlebellButton.WidthRequest = width;
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
                VisualStateManager.GoToState(MachineButton, "Normal");
                VisualStateManager.GoToState(KettlebellButton, "Normal");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum total weight (kg)";

                // Show relevant rows.
                BarWeightGrid.IsVisible = true;

                // Hide other rows.
                StartingWeightGrid.IsVisible = false;
                OneSideOnlyGrid.IsVisible = false;
                break;

            case ExerciseType.Dumbbell:
                // Update the button visual states.
                VisualStateManager.GoToState(DumbbellButton, "Selected");
                VisualStateManager.GoToState(BarbellButton, "Normal");
                VisualStateManager.GoToState(MachineButton, "Normal");
                VisualStateManager.GoToState(KettlebellButton, "Normal");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Max. weight per dumbbell (kg)";

                // Hide other rows.
                BarWeightGrid.IsVisible = false;
                StartingWeightGrid.IsVisible = false;
                OneSideOnlyGrid.IsVisible = false;
                break;

            case ExerciseType.Machine:
                // Update the button visual states.
                VisualStateManager.GoToState(MachineButton, "Selected");
                VisualStateManager.GoToState(BarbellButton, "Normal");
                VisualStateManager.GoToState(DumbbellButton, "Normal");
                VisualStateManager.GoToState(KettlebellButton, "Normal");

                // Update the label text.
                MaxWeightLabel.Text = "Maximum total weight (kg)";

                // Show relevant rows.
                StartingWeightGrid.IsVisible = true;
                OneSideOnlyGrid.IsVisible = true;

                // Hide other rows.
                BarWeightGrid.IsVisible = false;
                break;

            case ExerciseType.Kettlebell:
                // Update the button visual states.
                VisualStateManager.GoToState(KettlebellButton, "Selected");
                VisualStateManager.GoToState(BarbellButton, "Normal");
                VisualStateManager.GoToState(DumbbellButton, "Normal");
                VisualStateManager.GoToState(MachineButton, "Normal");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum weight (kg)";

                // Hide other rows.
                BarWeightGrid.IsVisible = false;
                StartingWeightGrid.IsVisible = false;
                OneSideOnlyGrid.IsVisible = false;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(exerciseType), exerciseType, null);
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

    private void OnMachineButtonClicked(object sender, EventArgs e)
    {
        UpdateExerciseType(ExerciseType.Machine);
    }

    private void OnKettlebellButtonClicked(object sender, EventArgs e)
    {
        UpdateExerciseType(ExerciseType.Kettlebell);
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
        await GetBars();

        // Initialise the items in the bar weight picker.
        var i = 0;
        var valueSelected = false;
        foreach (var (weight, bar) in _barLookup)
        {
            // Add the picker item.
            var weightString = weight.ToString(CultureInfo.InvariantCulture);
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
            var weightString = Bar.DefaultWeight.ToString(CultureInfo.InvariantCulture);
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

    private async Task<Dictionary<double, Bar>> GetBars()
    {
        if (_barLookup == null)
        {
            var bars = await BarRepository.GetAll(true);
            _barLookup = bars.ToDictionary(p => p.Weight, p => p);
        }
        return _barLookup;
    }

    private async Task<Dictionary<double, Plate>> GetPlates()
    {
        if (_plateLookup == null)
        {
            var plates = await PlateRepository.GetAll(true);
            _plateLookup = plates.ToDictionary(p => p.Weight, p => p);
        }
        return _plateLookup;
    }

    private async Task<Dictionary<double, Dumbbell>> GetDumbbells()
    {
        if (_dumbbellLookup == null)
        {
            var dumbbells = await DumbbellRepository.GetAll(true);
            _dumbbellLookup = dumbbells.ToDictionary(p => p.Weight, p => p);
        }
        return _dumbbellLookup;
    }

    private async Task<Dictionary<double, Kettlebell>> GetKettlebells()
    {
        if (_kettlebellLookup == null)
        {
            var kettlebells = await KettlebellRepository.GetAll(true);
            _kettlebellLookup = kettlebells.ToDictionary(p => p.Weight, p => p);
        }
        return _kettlebellLookup;
    }

    private async void OnCalculateButtonClicked(object sender, EventArgs e)
    {
        switch (_selectedExerciseType)
        {
            case ExerciseType.Barbell:
                // Get the max weight.
                if (!double.TryParse(MaxWeightEntry.Text, out _maxWeight))
                {
                    ErrorMessage.Text = "Please enter the maximum total weight, including the bar.";
                    return;
                }

                // Get the bar weight.
                _barWeight = double.Parse(BarWeightPicker.Items[BarWeightPicker.SelectedIndex]);

                // Get the available plate weights ordered from lightest to heaviest.
                await GetPlates();
                var availPlateWeights = _plateLookup.Keys.ToList();

                // Calculate and display the results.
                _barbellResults =
                    PlateSolver.CalculateResults(_maxWeight, _barWeight, true, availPlateWeights);
                DisplayBarbellResults();
                break;

            case ExerciseType.Dumbbell:
                // Get the max dumbbell weight from the calculator field.
                if (!double.TryParse(MaxWeightEntry.Text, out _maxWeight))
                {
                    ErrorMessage.Text = "Please enter the maximum weight per dumbbell.";
                    return;
                }

                // Get the available dumbbell weights ordered from lightest to heaviest.
                await GetDumbbells();
                var availDumbbells = _dumbbellLookup.Keys.ToList();

                // Calculate and display the results.
                _dumbbellResults = SingleWeightSolver.CalculateResults(_maxWeight, availDumbbells);
                DisplayDumbbellResults();
                break;

            case ExerciseType.Machine:
                // Get the max weight.
                if (!double.TryParse(MaxWeightEntry.Text, out _maxWeight))
                {
                    ErrorMessage.Text =
                        "Please enter the maximum total weight, including the machine's starting weight.";
                    return;
                }

                // Get the starting weight.
                if (!double.TryParse(StartingWeightEntry.Text, out _startingWeight))
                {
                    ErrorMessage.Text = "Please enter the machine's starting weight.";
                    return;
                }

                // Get if they want one side only.
                _oneSideOnly = OneSideOnlyCheckbox.IsChecked;

                // Get the available plate weights ordered from lightest to heaviest.
                await GetPlates();
                var availPlateWeights2 = _plateLookup.Keys.ToList();

                // Calculate and display the results.
                _machineResults = PlateSolver.CalculateResults(_maxWeight, _startingWeight,
                    _oneSideOnly, availPlateWeights2);
                DisplayMachineResults();
                break;

            case ExerciseType.Kettlebell:
                // Get the max kettlebell weight from the calculator field.
                if (!double.TryParse(MaxWeightEntry.Text, out _maxWeight))
                {
                    ErrorMessage.Text = "Please enter the maximum kettlebell weight.";
                    return;
                }

                // Get the available kettlebell weights ordered from lightest to heaviest.
                await GetKettlebells();
                var availKettlebells = _kettlebellLookup.Keys.ToList();

                // Calculate and display the results.
                _kettlebellResults =
                    SingleWeightSolver.CalculateResults(_maxWeight, availKettlebells);
                DisplayKettlebellResults();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DisplayPlateResults(Dictionary<double, List<double>> results,
        double startingWeight, bool oneSideOnly, string totalHeadingText)
    {
        // Clear the error message.
        ErrorMessage.Text = "";

        // Clear the results.
        MauiUtilities.ClearStack(CalculatorResults);

        // Check if there aren't any results to render.
        if (results == null)
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

        var maxPlateWeight = _plateLookup.Keys.Max();

        foreach (var (percent, platesResult) in results)
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
                FormattedText = TextUtility.StyleText(totalHeadingText, headerStyle),
            };
            textGrid.Add(totalHeading, 0, 1);

            // Ideal total weight.
            var idealTotal = _maxWeight * percent / 100.0;
            var idealPlates = (idealTotal - startingWeight) / (oneSideOnly ? 2 : 1);
            var idealTotalValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{idealTotal:F2} kg", weightStyle),
            };
            textGrid.Add(idealTotalValue, 1, 1);

            // Closest total weight.
            var closestPlates = platesResult.Sum();
            var closestTotal = (oneSideOnly ? 2 : 1) * closestPlates + startingWeight;
            var closestTotalValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{closestTotal:F2} kg", weightStyle),
            };
            textGrid.Add(closestTotalValue, 2, 1);

            ///////////
            // Row 2.

            // Plates heading.
            var platesHeadingText = oneSideOnly ? "Plates per end" : "Plates to select";
            var platesHeading = new Label
            {
                FormattedText = TextUtility.StyleText(platesHeadingText, headerStyle),
            };
            textGrid.Add(platesHeading, 0, 2);

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
                Padding = new Thickness(0, App.Spacing, 0, App.DoubleSpacing),
            };
            var j = 0;
            platesResult.Sort();
            foreach (var plateWeight in platesResult)
            {
                platesGrid.RowDefinitions.Add(new RowDefinition());
                PlatesPage.AddPlateToGrid(_plateLookup[plateWeight], platesGrid, 0, j,
                    maxPlateWeight);
                j++;
            }
            CalculatorResults.Add(platesGrid);
        }

        // Horizontal rule.
        CalculatorResults.Add(GetHorizontalRule(availWidth));
    }

    private void DisplaySingleWeightResults(Dictionary<double, double> results,
        Func<double, GraphicsView> createGraphic)
    {
        // Clear the error message.
        ErrorMessage.Text = "";

        // Clear the results.
        MauiUtilities.ClearStack(CalculatorResults);

        // Check if there aren't any results to render.
        if (results == null)
        {
            return;
        }

        // Prepare the styles.
        var percentStyle = MauiUtilities.LookupStyle("ResultsTablePercent");
        var headerStyle = MauiUtilities.LookupStyle("ResultsTableHeader");
        var weightStyle = MauiUtilities.LookupStyle("ResultsTableWeight");

        // Get the available width in device-independent pixels.
        var availWidth = GetAvailWidth();

        foreach (var (percent, closestWeight) in results)
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

            // Ideal weight.
            var idealWeight = _maxWeight * percent / 100.0;
            var idealValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{idealWeight:F2} kg", weightStyle),
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            textGrid.Add(idealValue, 1, 1);

            // Add graphic.
            var graphic = createGraphic(closestWeight);
            textGrid.Add(graphic, 2, 1);

            CalculatorResults.Add(textGrid);
        }

        // Horizontal rule.
        CalculatorResults.Add(GetHorizontalRule(availWidth));
    }

    private void DisplayBarbellResults()
    {
        DisplayPlateResults(_barbellResults, _barWeight, true, "Total including bar");
    }

    private void DisplayMachineResults()
    {
        DisplayPlateResults(_machineResults, _startingWeight, _oneSideOnly,
            "Total including starting weight");
    }

    private void DisplayDumbbellResults()
    {
        var createGraphic = (double weight) =>
        {
            return GraphicsFactory.CreateDumbbellGraphic(_dumbbellLookup[weight]);
        };
        DisplaySingleWeightResults(_dumbbellResults, createGraphic);
    }

    private void DisplayKettlebellResults()
    {
        var createGraphic = (double weight) =>
        {
            return GraphicsFactory.CreateKettlebellGraphic(_kettlebellLookup[weight]);
        };
        DisplaySingleWeightResults(_kettlebellResults, createGraphic);
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
