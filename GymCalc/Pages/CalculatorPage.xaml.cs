using Galaxon.Core.Exceptions;
using GymCalc.Enums;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class CalculatorPage : ContentPage
{
    #region Fields

    private readonly CalculatorViewModel? _viewModel;

    private bool _databaseInitialized;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    public CalculatorPage(CalculatorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    #endregion Constructor

    #region Events

    /// <inheritdoc/>
    protected override async void OnAppearing()
    {
        if (_viewModel == null)
        {
            return;
        }

        // Initialize database on first page load.
        if (!_databaseInitialized)
        {
            await _viewModel.InitializeDatabase();
            _databaseInitialized = true;
        }

        // Initialise the exercise type buttons.
        SetExerciseType(_viewModel.SelectedExerciseType);

        // Initialize other form elements.
        await _viewModel.Initialize();
    }

    private void OnBarbellButtonClicked(object sender, EventArgs e)
    {
        SetExerciseType(EExerciseType.Barbell);
    }

    private void OnDumbbellButtonClicked(object sender, EventArgs e)
    {
        SetExerciseType(EExerciseType.Dumbbell);
    }

    private void OnMachineButtonClicked(object sender, EventArgs e)
    {
        SetExerciseType(EExerciseType.Machine);
    }

    private void OnKettlebellButtonClicked(object sender, EventArgs e)
    {
        SetExerciseType(EExerciseType.Kettlebell);
    }

    private void OnBarbellTypePlateLoadedSelected(object sender, EventArgs e)
    {
        if (sender is RadioButton rb && BarWeightGrid != null)
        {
            BarWeightGrid.IsVisible = rb.IsChecked;
        }
    }

    private void OnBarbellTypeFixedSelected(object sender, EventArgs e)
    {
        if (sender is RadioButton rb && BarWeightGrid != null)
        {
            BarWeightGrid.IsVisible = !rb.IsChecked;
        }
    }

    private void OnMovementTypeChanged(object sender, EventArgs e)
    {
        if (_viewModel == null)
        {
            return;
        }

        _viewModel.StartingWeightLabel = _viewModel.MovementType == EMovementType.Isolateral
            ? "Starting weight per side"
            : "Starting weight";
    }

    #endregion Events

    #region UI

    private void SetExerciseType(EExerciseType exerciseType)
    {
        if (_viewModel == null)
        {
            return;
        }

        // Clear the error message.
        _viewModel.ErrorMessage = "";

        // Update the viewmodel.
        _viewModel.SelectedExerciseType = exerciseType;

        // Deselect all exercise type buttons.
        VisualStateManager.GoToState(BarbellButton, "Normal");
        VisualStateManager.GoToState(DumbbellButton, "Normal");
        VisualStateManager.GoToState(MachineButton, "Normal");
        VisualStateManager.GoToState(KettlebellButton, "Normal");

        // Update the button states.
        switch (exerciseType)
        {
            case EExerciseType.Barbell:
                // Update the button visual state.
                VisualStateManager.GoToState(BarbellButton, "Selected");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum total weight";

                // Hide/show rows.
                BarbellTypeGrid.IsVisible = true;
                BarWeightGrid.IsVisible = _viewModel.BarbellType == EBarbellType.PlateLoaded;
                MovementTypeGrid.IsVisible = false;
                StartingWeightGrid.IsVisible = false;
                break;

            case EExerciseType.Dumbbell:
                // Update the button visual state.
                VisualStateManager.GoToState(DumbbellButton, "Selected");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum weight per dumbbell";

                // Hide/show rows.
                BarbellTypeGrid.IsVisible = false;
                BarWeightGrid.IsVisible = false;
                MovementTypeGrid.IsVisible = false;
                StartingWeightGrid.IsVisible = false;
                break;

            case EExerciseType.Machine:
                // Update the button visual state.
                VisualStateManager.GoToState(MachineButton, "Selected");

                // Update the label text.
                MaxWeightLabel.Text = "Maximum total weight";

                // Hide/show rows.
                BarbellTypeGrid.IsVisible = false;
                BarWeightGrid.IsVisible = false;
                MovementTypeGrid.IsVisible = true;
                StartingWeightGrid.IsVisible = true;
                break;

            case EExerciseType.Kettlebell:
                // Update the button visual state.
                VisualStateManager.GoToState(KettlebellButton, "Selected");

                // Update the max weight label text.
                MaxWeightLabel.Text = "Maximum weight";

                // Hide/show rows.
                BarbellTypeGrid.IsVisible = false;
                BarWeightGrid.IsVisible = false;
                MovementTypeGrid.IsVisible = false;
                StartingWeightGrid.IsVisible = false;
                break;

            default:
                throw new MatchNotFoundException("Invalid exercise type.");
        }
    }

    #endregion UI
}
