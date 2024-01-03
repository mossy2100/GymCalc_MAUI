using AsyncAwaitBestPractices.MVVM;
using Galaxon.Core.Exceptions;
using GymCalc.Enums;
using GymCalc.Models;
using GymCalc.Repositories;
using GymCalc.Services;

namespace GymCalc.ViewModels;

public class EditViewModel : BaseViewModel
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database"></param>
    public EditViewModel(Database database)
    {
        // Dependencies.
        _database = database;

        // Commands.
        CancelCommand = new AsyncCommand(Cancel);
        SaveCommand = new AsyncCommand(Save);
    }

    #region Fields

    private readonly Database _database;

    private bool _enabled;

    private string? _errorMessage;

    private GymObject? _gymObject;

    private int _gymObjectId;

    private string? _gymObjectTypeName;

    private string? _color;

    private string? _operation;

    private string? _title;

    private EUnits _units;

    private bool _bandsOptionVisible;

    private EBandsOption _bandsOption;

    private string? _weightText;

    private IGymObjectRepository? _repo;

    #endregion Fields

    #region Bindable properties

    public string? Title
    {
        get => _title;

        set => SetProperty(ref _title, value);
    }

    public string? WeightText
    {
        get => _weightText;

        set => SetProperty(ref _weightText, value);
    }

    public EUnits Units
    {
        get => _units;

        set => SetProperty(ref _units, value);
    }

    public bool Enabled
    {
        get => _enabled;

        set => SetProperty(ref _enabled, value);
    }

    public string? Color
    {
        get => _color;

        set => SetProperty(ref _color, value);
    }

    public string ColorLabel => GeoService.GetSpelling("Color");

    public string? ErrorMessage
    {
        get => _errorMessage;

        set => SetProperty(ref _errorMessage, value);
    }

    public bool BandsOptionVisible
    {
        get => _bandsOptionVisible;

        set => SetProperty(ref _bandsOptionVisible, value);
    }

    public EBandsOption BandsOption
    {
        get => _bandsOption;

        set => SetProperty(ref _bandsOption, value);
    }

    #endregion Bindable properties

    #region Commands

    public ICommand CancelCommand { get; init; }

    public ICommand SaveCommand { get; init; }

    #endregion Commands

    #region Command methods

    /// <summary>
    /// Go back to the list page, showing items of the current type.
    /// </summary>
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Save the item to the database, then go back to the list page.
    /// </summary>
    private async Task Save()
    {
        // Make sure we have a reference to the repository.
        if (_repo == null)
        {
            return;
        }

        // Validate the form.
        bool weightOk = decimal.TryParse(WeightText, out decimal weight) && weight > 0;
        if (!weightOk)
        {
            ErrorMessage = "Please ensure the weight is a number greater than 0.";
            return;
        }

        ErrorMessage = "";

        // If this is an add operation, create new gym object.
        if (_operation == "add" || _gymObject == null)
        {
            _gymObject = _repo.Create();
        }

        // Copy values from the viewmodel to the model.
        _gymObject.Weight = weight;
        _gymObject.Units = Units;
        _gymObject.Enabled = Enabled;
        _gymObject.Color = Color;

        // Special handling for kettlebells.
        if (_gymObject is Kettlebell kettlebell)
        {
            kettlebell.BandsOption = BandsOption;
        }

        // Update the database.
        await _repo.Upsert(_gymObject);

        // Go back to the list of this object type.
        await Shell.Current.GoToAsync("..");
    }

    #endregion Command methods

    #region Other methods

    /// <summary>
    /// Hide and show the form fields appropriate to this object type.
    /// </summary>
    internal async Task Initialize(string? operation, string? gymObjectTypeName, int gymObjectId)
    {
        // Don't do anything unless all required parameters have been set.
        if (string.IsNullOrWhiteSpace(operation) || string.IsNullOrWhiteSpace(gymObjectTypeName)
            || (operation == "edit" && gymObjectId == 0))
        {
            return;
        }

        // Check for valid operation.
        if (operation != "add" && operation != "edit")
        {
            throw new ArgumentOutOfRangeException(nameof(operation),
                $"Invalid operation '{operation}'. Must be 'add' or 'edit'.");
        }

        // Remember the view parameters.
        _operation = operation;
        _gymObjectTypeName = gymObjectTypeName;
        _gymObjectId = gymObjectId;

        // Get the repository for this gym object type.
        _repo = _database.GetRepo(_gymObjectTypeName);

        // Set the title.
        TextInfo ti = new CultureInfo("en-US", false).TextInfo;
        Title = $"{ti.ToTitleCase(operation)} {_gymObjectTypeName}";

        // Reset the form.
        ResetForm();

        // If editing an existing gym object, load it from the database and populate the form.
        if (operation == "edit")
        {
            // This can throw a KeyNotFoundException if the gym object id is invalid.
            await LoadGymObjectIntoForm();
        }
        else
        {
            _gymObject = null;
        }
    }

    /// <summary>
    /// Reset the form values in preparation for loading a new item.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ResetForm()
    {
        WeightText = "";
        Units = UnitsService.GetDefaultUnits();
        Enabled = true;
        Color = "Black";
        BandsOptionVisible = false;
        BandsOption = EBandsOption.None;
    }

    /// <summary>
    /// Load an existing gym object from the database.
    /// </summary>
    /// <exception cref="MatchNotFoundException">
    /// If the gym object type is invalid (should never happen).
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// If the gym object with the given type and id is not found in the database.
    /// </exception>
    private async Task LoadGymObjectIntoForm()
    {
        // Guards.
        if (string.IsNullOrEmpty(_gymObjectTypeName))
        {
            throw new InvalidOperationException("Gym object type not specified.");
        }
        if (_gymObjectId == 0)
        {
            throw new InvalidOperationException("Gym object id not specified.");
        }
        if (_repo == null)
        {
            throw new InvalidOperationException("Repository not specified.");
        }

        // Load the object.
        _gymObject = await _repo.LoadById(_gymObjectId);

        if (_gymObject == null)
        {
            throw new DataException(
                $"{_gymObjectTypeName} object with Id = {_gymObjectId} was not found.");
        }

        // Set the form values.
        WeightText = _gymObject.Weight.ToString(CultureInfo.InvariantCulture);
        Units = _gymObject.Units;
        Enabled = _gymObject.Enabled;
        Color = _gymObject.Color;

        // Set the type-specific form values.
        if (_gymObject is Kettlebell kettlebell)
        {
            BandsOptionVisible = true;
            BandsOption = kettlebell.BandsOption;
        }
        else
        {
            BandsOptionVisible = false;
        }
    }

    #endregion Other methods
}
