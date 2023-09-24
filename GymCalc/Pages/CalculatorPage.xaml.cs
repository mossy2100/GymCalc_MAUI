using GymCalc.Constants;
using GymCalc.Utilities;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class CalculatorPage : ContentPage
{
    private readonly CalculatorViewModel _model;

    private bool _databaseInitialized;

    private bool _layoutInitialized;

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
    }

    #region Event handlers

    /// <summary>
    /// This runs whenever the page is displayed, and also when the orientation changes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSizeChanged(object sender, EventArgs e)
    {
        UpdateLayoutOrientation();
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        // Initialize database on first page load.
        if (!_databaseInitialized)
        {
            await _model.InitializeDatabase();
            _databaseInitialized = true;
        }

        // Set the user's preferred units, which may have changed on the settings page.
        var units = Units.GetPreferred();
        MaxWeightUnit.Text = units;
        BarWeightUnit.Text = units;
        StartingWeightUnit.Text = units;

        // Initialise the exercise type buttons.
        SetExerciseType(ExerciseType.Barbell);

        // Update the bar weight picker whenever this page appears, because the bar weights may have
        // changed on the Bars page.
        await _model.ResetBarWeightPicker();
    }

    private void OnBarbellButtonClicked(object sender, EventArgs e)
    {
        SetExerciseType(ExerciseType.Barbell);
    }

    private void OnDumbbellButtonClicked(object sender, EventArgs e)
    {
        SetExerciseType(ExerciseType.Dumbbell);
    }

    private void OnMachineButtonClicked(object sender, EventArgs e)
    {
        SetExerciseType(ExerciseType.Machine);
    }

    private void OnKettlebellButtonClicked(object sender, EventArgs e)
    {
        SetExerciseType(ExerciseType.Kettlebell);
    }

    #endregion Event handlers

    #region UI

    private void UpdateLayoutOrientation()
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
        if (_model.PlatesResultsVisible || _model.SingleWeightResultsVisible)
        {
            switch (_model.SelectedExerciseType)
            {
                case ExerciseType.Barbell:
                    // await DisplayBarbellResults();
                    break;

                case ExerciseType.Dumbbell:
                    // await DisplayDumbbellResults();
                    break;

                case ExerciseType.Machine:
                    // await DisplayMachineResults();
                    break;

                case ExerciseType.Kettlebell:
                    // await DisplayKettlebellResults();
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

    private void SetExerciseType(ExerciseType exerciseType)
    {
        // Clear the error message.
        _model.ErrorMessage = "";

        // Update the field.
        _model.SelectedExerciseType = exerciseType;

        // Deselect all exercise type buttons.
        VisualStateManager.GoToState(BarbellButton, "Normal");
        VisualStateManager.GoToState(DumbbellButton, "Normal");
        VisualStateManager.GoToState(MachineButton, "Normal");
        VisualStateManager.GoToState(KettlebellButton, "Normal");

        // Update the button states.
        switch (exerciseType)
        {
            case ExerciseType.Barbell:
                // Update the button visual state.
                VisualStateManager.GoToState(BarbellButton, "Selected");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum total weight";

                // Show relevant rows.
                BarWeightGrid.IsVisible = true;

                // Hide other rows.
                StartingWeightGrid.IsVisible = false;
                OneSideOnlyGrid.IsVisible = false;
                break;

            case ExerciseType.Dumbbell:
                // Update the button visual state.
                VisualStateManager.GoToState(DumbbellButton, "Selected");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum weight per dumbbell";

                // Hide other rows.
                BarWeightGrid.IsVisible = false;
                StartingWeightGrid.IsVisible = false;
                OneSideOnlyGrid.IsVisible = false;
                break;

            case ExerciseType.Machine:
                // Update the button visual state.
                VisualStateManager.GoToState(MachineButton, "Selected");

                // Update the label text.
                MaxWeightLabel.Text = "Maximum total weight";

                // Show relevant rows.
                StartingWeightGrid.IsVisible = true;
                OneSideOnlyGrid.IsVisible = true;

                // Hide other rows.
                BarWeightGrid.IsVisible = false;
                break;

            case ExerciseType.Kettlebell:
                // Update the button visual state.
                VisualStateManager.GoToState(KettlebellButton, "Selected");

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

    // private async Task ScrollToResults()
    // {
    //     var y = MauiUtilities.GetOrientation() == DisplayOrientation.Portrait && _resultsDisplayed
    //         ? CalculatorResults.Y
    //         : 0;
    //     await CalculatorScrollView.ScrollToAsync(0, y, true);
    // }

    #endregion UI

    #region Display results

    // private void ClearResults()
    // {
    //     // MauiUtilities.ClearLayout(CalculatorResults);
    //     CalculatorResults.HeightRequest = -1;
    //     _resultsDisplayed = false;
    // }

    // private void SetErrorMessage(string errorMessage)
    // {
    //     ErrorMessage.Text = errorMessage;
    //     ErrorMessage.IsVisible = true;
    // }

    // private async Task DisplayBarbellResults()
    // {
    //     await DisplayPlateResults(_model.BarbellResults, _model.BarWeight, true, "Total",
    //         "Plates per end");
    // }

    // private async Task DisplayMachineResults()
    // {
    //     await DisplayPlateResults(_model.MachineResults, _model.StartingWeight!.Value,
    //         _model.OneSideOnly, "Total", "Plates per side");
    // }

    // private async Task DisplayDumbbellResults()
    // {
    //     await DisplaySingleWeightResults(_model.DumbbellResults, weight =>
    //     {
    //         var drawable = new DumbbellDrawable { GymObject = _model.Dumbbells[weight] };
    //         return drawable.CreateGraphicsView();
    //     });
    // }

    // private async Task DisplayKettlebellResults()
    // {
    //     await DisplaySingleWeightResults(_model.KettlebellResults, weight =>
    //     {
    //         var drawable = new KettlebellDrawable { GymObject = _model.Kettlebells[weight] };
    //         return drawable.CreateGraphicsView();
    //     });
    // }

    /*
    private async Task DisplayPlateResults(List<PlatesResult> results, double startingWeight,
        bool oneSideOnly, string totalHeadingText, string platesPerSideText)
    {
        ClearErrorMessage();

        // Clear the results.
        // ClearResults();

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

        var maxPlateWeight = _model.Plates.Keys.Max();

        foreach (var result in results)
        {
            // Horizontal rule.
            // CalculatorResults.Add(TextUtility.GetHorizontalRule(availWidth));

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
                FormattedText = TextUtility.StyleText($"{result.Percent}%", percentStyle),
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
            var idealTotal = _model.MaxWeight * result.Percent / 100.0;
            var idealPlates = (idealTotal - startingWeight) / (oneSideOnly ? 2 : 1);
            var idealTotalValue = new Label
            {
                FormattedText =
                    TextUtility.StyleText($"{idealTotal:F2} {CalculatorViewModel.Units}",
                        weightStyle),
            };
            textGrid.Add(idealTotalValue, 1, 1);

            // Closest total weight.
            var closestPlates = result.Sum(go => go.Weight);
            var closestTotal = (oneSideOnly ? 2 : 1) * closestPlates + startingWeight;
            var closestTotalValue = new Label
            {
                FormattedText =
                    TextUtility.StyleText($"{closestTotal:F2} {CalculatorViewModel.Units}",
                        focusWeightStyle),
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
                FormattedText =
                    TextUtility.StyleText($"{idealPlates:F2} {CalculatorViewModel.Units}",
                        weightStyle),
            };
            textGrid.Add(idealPlatesValue, 1, 2);

            // Closest plates weight.
            var closestPlatesValue = new Label
            {
                FormattedText =
                    TextUtility.StyleText($"{closestPlates:F2} {CalculatorViewModel.Units}",
                        focusWeightStyle),
            };
            textGrid.Add(closestPlatesValue, 2, 2);

            // CalculatorResults.Add(textGrid);

            // If we have some plates, construct the plates grid and add it to the layout.
            if (result.Count > 0)
            {
                var platesGrid = new Grid();
                var j = 0;
                result.Sort();
                foreach (var plate in result)
                {
                    platesGrid.RowDefinitions.Add(new RowDefinition());

                    // Add the plate graphic.
                    var drawable = new PlateDrawable
                    {
                        GymObject = plate, //_model.Plates[plate.Weight],
                        MaxWidth = maxWidth,
                        MaxWeight = maxPlateWeight,
                    };
                    var plateGraphic = drawable.CreateGraphicsView();
                    platesGrid.Add(plateGraphic, 0, j);
                    j++;
                }
                // CalculatorResults.Add(platesGrid);
            }
        }

        // Horizontal rule.
        // CalculatorResults.Add(TextUtility.GetHorizontalRule(availWidth));

        // Scroll to results.
        ResetFormPadding();
        _resultsDisplayed = true;
        // await ScrollToResults();
    }

    private async Task DisplaySingleWeightResults(Dictionary<double, double> results,
        Func<double, GraphicsView> createGraphic)
    {
        ClearErrorMessage();

        // Clear the results.
        // ClearResults();

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
            // CalculatorResults.Add(TextUtility.GetHorizontalRule(availWidth));

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
                FormattedText =
                    TextUtility.StyleText($"{idealWeight:F2} {CalculatorViewModel.Units}",
                        weightStyle),
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            textGrid.Add(idealValue, 1, 1);

            // Add graphic.
            var graphic = createGraphic(closestWeight);
            textGrid.Add(graphic, 2, 1);

            // CalculatorResults.Add(textGrid);
        }

        // Horizontal rule.
        // CalculatorResults.Add(TextUtility.GetHorizontalRule(availWidth));

        // Scroll to results.
        ResetFormPadding();
        _resultsDisplayed = true;
        // await ScrollToResults();
    }
    */

    #endregion Display results
}
