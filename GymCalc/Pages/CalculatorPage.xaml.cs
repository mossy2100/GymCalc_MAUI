using Galaxon.Core.Exceptions;
using GymCalc.Enums;
using GymCalc.ViewModels;
using InputKitRadioButton = InputKit.Shared.Controls.RadioButton;

namespace GymCalc.Pages;

public partial class CalculatorPage : ContentPage
{
    #region Fields

    private readonly CalculatorViewModel _model;

    private bool _databaseInitialized;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    public CalculatorPage(CalculatorViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
    }

    #endregion Constructor

    #region Events

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
        SetExerciseType(_model.SelectedExerciseType);

        // Initialize other form elements.
        await _model.Initialize();
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
        if (sender is InputKitRadioButton rb && BarWeightGrid != null)
        {
            BarWeightGrid.IsVisible = rb.IsChecked;
        }
    }

    private void OnBarbellTypeFixedSelected(object sender, EventArgs e)
    {
        if (sender is InputKitRadioButton rb && BarWeightGrid != null)
        {
            BarWeightGrid.IsVisible = !rb.IsChecked;
        }
    }

    #endregion Events

    #region UI

    private void SetExerciseType(EExerciseType exerciseType)
    {
        // Clear the error message.
        _model.ErrorMessage = "";

        // Update the viewmodel.
        _model.SelectedExerciseType = exerciseType;

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
                BarWeightGrid.IsVisible = _model.BarbellType == EBarbellType.PlateLoaded;
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
