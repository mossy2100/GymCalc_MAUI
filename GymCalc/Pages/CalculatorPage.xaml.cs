using System.Globalization;
using GymCalc.Calculations;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Constants;
using GymCalc.Graphics;
using GymCalc.Graphics.Drawables;
using GymCalc.Utilities;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class CalculatorPage : ContentPage
{
    private readonly CalculatorViewModel _model;

    private bool _databaseInitialized;

    private bool _layoutInitialized;

    private bool _resultsDisplayed;

    private const int _MaxKeyboardHeight = 240;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Values extracted from user preferences.
    private string _units;

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
    public CalculatorPage(CalculatorViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;

        // Events.
        SizeChanged += OnSizeChanged;
        // Workaround for issue with keyboard overlapping bottom of form when there are no results.
        MaxWeight.Focused += Entry_Focused;
        MaxWeight.Unfocused += Entry_Unfocused;
        StartingWeight.Focused += Entry_Focused;
        StartingWeight.Unfocused += Entry_Unfocused;
    }

    #region Event handlers

    /// <summary>
    /// This runs whenever the page is displayed, and also when the orientation changes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSizeChanged(object sender, EventArgs e)
    {
        await UpdateLayoutOrientation();
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

        // Set the user's preferred units, which may have changed on the settings page.
        _units = Units.GetPreferred();
        MaxWeightUnit.Text = _units;
        BarWeightUnit.Text = _units;
        StartingWeightUnit.Text = _units;

        // Invalidate collections because they may have changed on one of the other pages.
        _barLookup = null;
        _plateLookup = null;
        _dumbbellLookup = null;
        _kettlebellLookup = null;

        // Initialise the exercise type buttons.
        UpdateExerciseType(ExerciseType.Barbell);

        // Update the bar weight picker whenever this page appears, because the bar weights may have
        // changed on the Bars page.
        await ResetBarWeightPicker();
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

    private async void OnCalculateButtonClicked(object sender, EventArgs e)
    {
        switch (_model.SelectedExerciseType)
        {
            case ExerciseType.Barbell:
                await DoBarbellCalculations();
                break;

            case ExerciseType.Dumbbell:
                await DoDumbbellCalculations();
                break;

            case ExerciseType.Machine:
                await DoMachineCalculations();
                break;

            case ExerciseType.Kettlebell:
                await DoKettlebellCalculations();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(_model.SelectedExerciseType),
                    "Invalid exercise type.");
        }
    }

    #endregion Event handlers

    #region Calculations

    private bool ValidateMaxWeight()
    {
        // // Get the max weight.
        // var maxWeightOk = double.TryParse(MaxWeight.Text, out var maxWeight) && maxWeight > 0;
        // if (!maxWeightOk)
        // {
        //     SetErrorMessage("Please enter the maximum weight as a number greater than 0.");
        //     return false;
        // }
        //
        // // Set the field.
        // _maxWeight = maxWeight;

        // Rewrite the entry value in case it has leading or trailing zeros, spaces, etc.
        // MaxWeight.Text = _model.MaxWeight.ToString(CultureInfo.InvariantCulture);

        return true;
    }

    private async Task DoBarbellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Get the bar weight.
        // _barWeight = double.Parse(BarWeight.Items[BarWeight.SelectedIndex]);

        // Get the available plate weights ordered from lightest to heaviest.
        await LoadPlates();
        var availPlateWeights = _plateLookup.Keys.ToList();

        // Calculate and display the results.
        _barbellResults =
            PlateSolver.CalculateResults(_model.MaxWeight, _model.BarWeight, true, availPlateWeights);
        await DisplayBarbellResults();
    }

    private async Task DoDumbbellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Get the available dumbbell weights ordered from lightest to heaviest.
        await LoadDumbbells();
        var availDumbbells = _dumbbellLookup.Keys.ToList();

        // Calculate and display the results.
        _dumbbellResults = SingleWeightSolver.CalculateResults(_model.MaxWeight, availDumbbells);
        await DisplayDumbbellResults();
    }

    private async Task DoMachineCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Get and validate the starting weight. If it's blank, default to 0.
        double startingWeight;
        if (string.IsNullOrWhiteSpace(StartingWeight.Text))
        {
            startingWeight = 0;
        }
        else if (!double.TryParse(StartingWeight.Text, out startingWeight)
            || startingWeight < 0)
        {
            SetErrorMessage(
                "Please enter the machine's starting weight as a number greater than or equal to 0, or just leave it blank.");
            return;
        }

        // Rewrite the starting weight field value.
        StartingWeight.Text = startingWeight.ToString(CultureInfo.InvariantCulture);

        // Check if they want one side only.
        // bool oneSideOnly = OneSideOnly.IsChecked;

        // Get the available plate weights ordered from lightest to heaviest.
        await LoadPlates();
        var availPlateWeights2 = _plateLookup.Keys.ToList();

        // Calculate and display the results.
        _machineResults = PlateSolver.CalculateResults(_model.MaxWeight, startingWeight,
            _model.OneSideOnly, availPlateWeights2);
        await DisplayMachineResults();
    }

    private async Task DoKettlebellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Get the available kettlebell weights ordered from lightest to heaviest.
        await LoadKettlebells();
        var availKettlebells = _kettlebellLookup.Keys.ToList();

        // Calculate and display the results.
        _kettlebellResults =
            SingleWeightSolver.CalculateResults(_model.MaxWeight, availKettlebells);
        await DisplayKettlebellResults();
    }

    #endregion Calculations

    #region UI

    /// <summary>
    /// Reset the bar weight picker items.
    /// </summary>
    private async Task ResetBarWeightPicker()
    {
        // Get the current selected value.
        var initialSelectedIndex = BarWeight.SelectedIndex;
        var initialSelectedValue = initialSelectedIndex != -1
            ? BarWeight.Items[initialSelectedIndex]
            : null;

        // Reset the picker items.
        BarWeight.Items.Clear();
        BarWeight.SelectedIndex = -1;

        // Get the available bar weights ordered by weight.
        await LoadBars();

        // Initialise the items in the bar weight picker.
        var i = 0;
        var valueSelected = false;
        foreach (var (weight, bar) in _barLookup)
        {
            // Add the picker item.
            var weightString = weight.ToString(CultureInfo.InvariantCulture);
            BarWeight.Items.Add(weightString);

            // Try to select the same weight that was selected before.
            if (!valueSelected && weightString == initialSelectedValue)
            {
                BarWeight.SelectedIndex = i;
                valueSelected = true;
            }

            i++;
        }

        // If the original selected bar weight is no longer present, try to select the default.
        if (!valueSelected)
        {
            var weightString = BarRepository.DefaultWeight.ToString(CultureInfo.InvariantCulture);
            for (i = 0; i < BarWeight.Items.Count; i++)
            {
                // Default selection.
                if (BarWeight.Items[i] == weightString)
                {
                    BarWeight.SelectedIndex = i;
                    valueSelected = true;
                    break;
                }
            }
        }

        // If no bar weight has been selected yet, select the first one.
        if (!valueSelected)
        {
            BarWeight.SelectedIndex = 0;
        }
    }

    private async Task UpdateLayoutOrientation()
    {
        // Get the device orientation.
        var newOrientation = MauiUtilities.GetOrientation() == DisplayOrientation.Landscape
            ? StackOrientation.Horizontal
            : StackOrientation.Vertical;

        // Skip the redraw if we don't need to do it.
        if (_layoutInitialized && newOrientation == CalculatorLayout.Orientation)
        {
            return;
        }

        // If different, update the layout.
        CalculatorLayout.Orientation = newOrientation;

        // Update the button widths.
        ResetExerciseTypeButtonWidths();

        // If there are any results, re-render them for the altered width.
        if (_resultsDisplayed)
        {
            switch (_model.SelectedExerciseType)
            {
                case ExerciseType.Barbell:
                    await DisplayBarbellResults();
                    break;

                case ExerciseType.Dumbbell:
                    await DisplayDumbbellResults();
                    break;

                case ExerciseType.Machine:
                    await DisplayMachineResults();
                    break;

                case ExerciseType.Kettlebell:
                    await DisplayKettlebellResults();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_model.SelectedExerciseType),
                        "Invalid exercise type.");
            }
        }

        _layoutInitialized = true;
    }

    private double GetAvailWidth()
    {
        var scrollViewWidth = CalculatorScrollView.Width - CalculatorScrollView.Padding.Left
            - CalculatorScrollView.Padding.Right;
        return MauiUtilities.GetOrientation() == DisplayOrientation.Landscape
            ? (scrollViewWidth - CalculatorLayout.Spacing) / 2
            : scrollViewWidth;
    }

    private void ResetExerciseTypeButtonWidths()
    {
        var width = (GetAvailWidth() - ExerciseTypeButtonGrid.ColumnSpacing) / 2;
        BarbellButton.WidthRequest = width;
        DumbbellButton.WidthRequest = width;
        MachineButton.WidthRequest = width;
        KettlebellButton.WidthRequest = width;
    }

    private void UpdateExerciseType(ExerciseType exerciseType)
    {
        ClearErrorMessage();

        // Update the field.
        _model.SelectedExerciseType = exerciseType;

        // Update the button states.
        switch (exerciseType)
        {
            case ExerciseType.Barbell:
                // Update the button visual states.
                VisualStateManager.GoToState(BarbellButton, "Selected");
                VisualStateManager.GoToState(DumbbellButton, "Normal");
                VisualStateManager.GoToState(MachineButton, "Normal");
                VisualStateManager.GoToState(KettlebellButton, "Normal");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum total weight";

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
                MaxWeightLabel.Text = "Maximum weight per dumbbell";

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
                MaxWeightLabel.Text = "Maximum total weight";

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
                MaxWeightLabel.Text = "Maximum weight";

                // Hide other rows.
                BarWeightGrid.IsVisible = false;
                StartingWeightGrid.IsVisible = false;
                OneSideOnlyGrid.IsVisible = false;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(exerciseType), exerciseType, null);
        }
    }

    private async Task ScrollToResults()
    {
        var y = MauiUtilities.GetOrientation() == DisplayOrientation.Portrait && _resultsDisplayed
            ? CalculatorResults.Y
            : 0;
        await CalculatorScrollView.ScrollToAsync(0, y, true);
    }

    #endregion UI

    #region Lookup tables

    private async Task LoadBars()
    {
        if (_barLookup == null)
        {
            var bars = await BarRepository.GetInstance().GetAll(_units, true);
            _barLookup = bars.ToDictionary(p => p.Weight, p => p);
        }
    }

    private async Task LoadPlates()
    {
        if (_plateLookup == null)
        {
            var plates = await PlateRepository.GetInstance().GetAll(_units, true);
            _plateLookup = plates.ToDictionary(p => p.Weight, p => p);
        }
    }

    private async Task LoadDumbbells()
    {
        if (_dumbbellLookup == null)
        {
            var dumbbells = await DumbbellRepository.GetInstance().GetAll(_units, true);
            _dumbbellLookup = dumbbells.ToDictionary(p => p.Weight, p => p);
        }
    }

    private async Task LoadKettlebells()
    {
        if (_kettlebellLookup == null)
        {
            var kettlebells = await KettlebellRepository.GetInstance().GetAll(_units, true);
            _kettlebellLookup = kettlebells.ToDictionary(p => p.Weight, p => p);
        }
    }

    #endregion Lookup tables

    #region Display results

    private void ClearResults()
    {
        MauiUtilities.ClearLayout(CalculatorResults);
        CalculatorResults.HeightRequest = -1;
        _resultsDisplayed = false;
    }

    private void ClearErrorMessage()
    {
        ErrorMessage.Text = "";
        ErrorMessage.IsVisible = false;
    }

    private void SetErrorMessage(string errorMessage)
    {
        ErrorMessage.Text = errorMessage;
        ErrorMessage.IsVisible = true;
    }

    private async Task DisplayBarbellResults()
    {
        await DisplayPlateResults(_barbellResults, _model.BarWeight, true, "Total", "Plates per end");
    }

    private async Task DisplayMachineResults()
    {
        await DisplayPlateResults(_machineResults, _model.StartingWeight, _model.OneSideOnly, "Total",
            "Plates per side");
    }

    private async Task DisplayDumbbellResults()
    {
        await LoadDumbbells();
        await DisplaySingleWeightResults(_dumbbellResults, weight =>
        {
            var drawable = new DumbbellDrawable { GymObject = _dumbbellLookup[weight] };
            return drawable.CreateGraphicsView();
        });
    }

    private async Task DisplayKettlebellResults()
    {
        await LoadKettlebells();
        await DisplaySingleWeightResults(_kettlebellResults, weight =>
        {
            var drawable = new KettlebellDrawable { GymObject = _kettlebellLookup[weight] };
            return drawable.CreateGraphicsView();
        });
    }

    private async Task DisplayPlateResults(Dictionary<double, List<double>> results,
        double startingWeight, bool oneSideOnly, string totalHeadingText, string platesPerSideText)
    {
        ClearErrorMessage();

        // Clear the results.
        ClearResults();

        // Check if there are any results to render.
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

        // Get the maximum plate width.
        var maxWidth = (int)(availWidth - 50);

        await LoadPlates();
        var maxPlateWeight = _plateLookup.Keys.Max();

        foreach (var (percent, platesResult) in results)
        {
            // Horizontal rule.
            CalculatorResults.Add(TextUtility.GetHorizontalRule(availWidth));

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
            var idealTotal = _model.MaxWeight * percent / 100.0;
            var idealPlates = (idealTotal - startingWeight) / (oneSideOnly ? 2 : 1);
            var idealTotalValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{idealTotal:F2} {_units}", weightStyle),
            };
            textGrid.Add(idealTotalValue, 1, 1);

            // Closest total weight.
            var closestPlates = platesResult.Sum();
            var closestTotal = (oneSideOnly ? 2 : 1) * closestPlates + startingWeight;
            var closestTotalValue = new Label
            {
                FormattedText =
                    TextUtility.StyleText($"{closestTotal:F2} {_units}", focusWeightStyle),
            };
            textGrid.Add(closestTotalValue, 2, 1);

            ///////////
            // Row 2.

            // Plates heading.
            var platesHeadingText = oneSideOnly ? platesPerSideText : "Plates to select";
            var platesHeading = new Label
            {
                FormattedText = TextUtility.StyleText(platesHeadingText, headerStyle),
            };
            textGrid.Add(platesHeading, 0, 2);

            // Ideal plates weight.
            var idealPlatesValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{idealPlates:F2} {_units}", weightStyle),
            };
            textGrid.Add(idealPlatesValue, 1, 2);

            // Closest plates weight.
            var closestPlatesValue = new Label
            {
                FormattedText =
                    TextUtility.StyleText($"{closestPlates:F2} {_units}", focusWeightStyle),
            };
            textGrid.Add(closestPlatesValue, 2, 2);

            CalculatorResults.Add(textGrid);

            // If we have some plates, construct the plates grid and add it to the layout.
            if (platesResult.Count > 0)
            {
                var platesGrid = new Grid();
                var j = 0;
                platesResult.Sort();
                foreach (var plateWeight in platesResult)
                {
                    platesGrid.RowDefinitions.Add(new RowDefinition());

                    // Add the plate graphic.
                    var drawable = new PlateDrawable
                    {
                        GymObject = _plateLookup[plateWeight],
                        MaxWidth = maxWidth,
                        MaxWeight = maxPlateWeight,
                    };
                    var plateGraphic = drawable.CreateGraphicsView();
                    platesGrid.Add(plateGraphic, 0, j);
                    j++;
                }
                CalculatorResults.Add(platesGrid);
            }
        }

        // Horizontal rule.
        CalculatorResults.Add(TextUtility.GetHorizontalRule(availWidth));

        // Scroll to results.
        ResetFormPadding();
        _resultsDisplayed = true;
        await ScrollToResults();
    }

    private async Task DisplaySingleWeightResults(Dictionary<double, double> results,
        Func<double, GraphicsView> createGraphic)
    {
        ClearErrorMessage();

        // Clear the results.
        ClearResults();

        // Check if there are any results to render.
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
            CalculatorResults.Add(TextUtility.GetHorizontalRule(availWidth));

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
                Padding = new Thickness(0, PageLayout.DoubleSpacing, 0, PageLayout.DoubleSpacing),
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
            var idealWeight = _model.MaxWeight * percent / 100.0;
            var idealValue = new Label
            {
                FormattedText = TextUtility.StyleText($"{idealWeight:F2} {_units}", weightStyle),
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
        CalculatorResults.Add(TextUtility.GetHorizontalRule(availWidth));

        // Scroll to results.
        ResetFormPadding();
        _resultsDisplayed = true;
        await ScrollToResults();
    }

    #endregion Display results

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        if (!_resultsDisplayed)
        {
            CalculatorForm.Padding = new Thickness(0, 0, 0, _MaxKeyboardHeight);
        }
    }

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        ResetFormPadding();
    }

    private void ResetFormPadding()
    {
        if (CalculatorForm.Padding.Bottom > 0)
        {
            CalculatorForm.Padding = new Thickness(0, 0, 0, 0);
        }
    }
}
