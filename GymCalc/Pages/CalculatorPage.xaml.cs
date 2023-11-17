using Galaxon.Core.Exceptions;
using Galaxon.Maui;
using GymCalc.Constants;
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

    /// <inheritdoc/>
    protected override async void OnAppearing()
    {
        // Initialize database on first page load.
        if (!_databaseInitialized)
        {
            await _model.InitializeDatabase();
            _databaseInitialized = true;
        }

        // Initialise the exercise type buttons.
        SetExerciseType(ExerciseType.Barbell);

        // Initialize other form elements.
        await _model.Initialize();
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
        var newOrientation = MauiUtility.GetOrientation() == DisplayOrientation.Landscape
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
            }
        }

        _layoutInitialized = true;
    }

    private double GetAvailWidth()
    {
        var scrollViewWidth = CalculatorScrollView.Width - CalculatorScrollView.Padding.Left
            - CalculatorScrollView.Padding.Right;
        return MauiUtility.GetOrientation() == DisplayOrientation.Landscape
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

        // Update the view model.
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

                // Hide/show rows.
                BarWeightGrid.IsVisible = true;
                MachineTypeGrid.IsVisible = false;
                StartingWeightGrid.IsVisible = false;
                break;

            case ExerciseType.Dumbbell:
                // Update the button visual state.
                VisualStateManager.GoToState(DumbbellButton, "Selected");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum weight per dumbbell";

                // Hide/show rows.
                BarWeightGrid.IsVisible = false;
                MachineTypeGrid.IsVisible = false;
                StartingWeightGrid.IsVisible = false;
                break;

            case ExerciseType.Machine:
                // Update the button visual state.
                VisualStateManager.GoToState(MachineButton, "Selected");

                // Update the label text.
                MaxWeightLabel.Text = "Maximum total weight";

                // Hide/show rows.
                BarWeightGrid.IsVisible = false;
                MachineTypeGrid.IsVisible = true;
                StartingWeightGrid.IsVisible = true;
                break;

            case ExerciseType.Kettlebell:
                // Update the button visual state.
                VisualStateManager.GoToState(KettlebellButton, "Selected");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum weight";

                // Hide/show rows.
                BarWeightGrid.IsVisible = false;
                MachineTypeGrid.IsVisible = false;
                StartingWeightGrid.IsVisible = false;
                break;

            default:
                throw new NoMatchingCaseException("Invalid exercise type.");
        }
    }

    // private async Task ScrollToResults()
    // {
    //     var y = MauiUtility.GetOrientation() == DisplayOrientation.Portrait && _resultsDisplayed
    //         ? CalculatorResults.Y
    //         : 0;
    //     await CalculatorScrollView.ScrollToAsync(0, y, true);
    // }

    #endregion UI
}
