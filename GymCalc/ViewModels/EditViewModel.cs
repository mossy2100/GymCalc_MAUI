using System.Data;
using System.Globalization;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using Galaxon.Core.Types;
using Galaxon.Core.Exceptions;
using GymCalc.Data;
using GymCalc.Models;
using GymCalc.Shared;

namespace GymCalc.ViewModels;

public class EditViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    // Dependencies.

    private readonly Database _database;

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly BarbellRepository _barbellRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    // ---------------------------------------------------------------------------------------------
    // Fields

    private string? _bandColor;

    private bool _enabled;

    private string? _errorMessage;

    private GymObject? _gymObject;

    private int _gymObjectId;

    private string? _gymObjectTypeName;

    private bool? _hasBands;

    private bool _hasBandsIsVisible;

    private string? _mainColor;

    private bool _mainColorIsVisible;

    private string? _operation;

    // ---------------------------------------------------------------------------------------------
    // Bindable properties.

    private string? _title;

    private string? _units;

    /// <summary>
    /// The weight (parsed from the WeightText entry field).
    /// </summary>
    private decimal _weight;

    private string? _weightText;

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database"></param>
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="barbellRepo"></param>
    /// <param name="dumbbellRepo"></param>
    /// <param name="kettlebellRepo"></param>
    public EditViewModel(Database database, BarRepository barRepo, PlateRepository plateRepo,
        BarbellRepository barbellRepo, DumbbellRepository dumbbellRepo,
        KettlebellRepository kettlebellRepo)
    {
        // Dependencies.
        _database = database;
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _barbellRepo = barbellRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;

        // Commands.
        CancelCommand = new AsyncCommand(Cancel);
        SaveCommand = new AsyncCommand(SaveGymObject);
    }

    // ---------------------------------------------------------------------------------------------
    // Commands.

    public ICommand CancelCommand { get; init; }

    public ICommand SaveCommand { get; init; }

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

    public string? Units
    {
        get => _units;

        set => SetProperty(ref _units, value);
    }

    public bool Enabled
    {
        get => _enabled;

        set => SetProperty(ref _enabled, value);
    }

    public string? MainColor
    {
        get => _mainColor;

        set => SetProperty(ref _mainColor, value);
    }

    public bool? HasBands
    {
        get => _hasBands;

        set => SetProperty(ref _hasBands, value);
    }

    public string? BandColor
    {
        get => _bandColor;

        set => SetProperty(ref _bandColor, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;

        set => SetProperty(ref _errorMessage, value);
    }

    public bool MainColorIsVisible
    {
        get => _mainColorIsVisible;

        set => SetProperty(ref _mainColorIsVisible, value);
    }

    public bool HasBandsIsVisible
    {
        get => _hasBandsIsVisible;

        set => SetProperty(ref _hasBandsIsVisible, value);
    }

    // ---------------------------------------------------------------------------------------------
    // Command methods.

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
    private async Task SaveGymObject()
    {
        // Validate the form.
        var weightOk = decimal.TryParse(WeightText, out var weight) && weight > 0;
        if (!weightOk)
        {
            ErrorMessage = "Please ensure the weight is a number greater than 0.";
            return;
        }
        _weight = weight;
        ErrorMessage = "";

        // Update the object. The exact process will vary by type.
        _gymObject = _gymObjectTypeName switch
        {
            nameof(Bar) => await SaveBar(),
            nameof(Plate) => await SavePlate(),
            nameof(Barbell) => await SaveBarbell(),
            nameof(Dumbbell) => await SaveDumbbell(),
            nameof(Kettlebell) => await SaveKettlebell(),
            _ => throw new Exception("Invalid gym object type.")
        };

        // Go back to the list of this object type.
        await Shell.Current.GoToAsync("..");
    }

    // ---------------------------------------------------------------------------------------------

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

        // Set the title.
        var ti = new CultureInfo("en-US", false).TextInfo;
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
    /// Reset the form in preparation for a new item.
    /// The object type affects what fields are displayed.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ResetForm()
    {
        WeightText = "";
        Units = UnitsUtility.GetDefault().GetDescription();
        Enabled = true;
        MainColor = "OffBlack";
        HasBands = false;
        BandColor = "OffBlack";

        // Hide/show certain fields according to the object type.
        switch (_gymObjectTypeName)
        {
            case nameof(Bar):
            case nameof(Barbell):
                MainColorIsVisible = false;
                HasBandsIsVisible = false;
                break;

            case nameof(Plate):
            case nameof(Dumbbell):
                MainColorIsVisible = true;
                HasBandsIsVisible = false;
                break;

            case nameof(Kettlebell):
                MainColorIsVisible = true;
                HasBandsIsVisible = true;
                break;

            default:
                throw new NoMatchingCaseException(
                    $"Invalid gym object type '{_gymObjectTypeName}'");
        }
    }

    /// <summary>
    /// Load an existing gym object from the database.
    /// </summary>
    /// <exception cref="NoMatchingCaseException">
    /// If the gym object type is invalid (should never happen).
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    ///If the gym object with the given type and id is not found in the database.
    /// </exception>
    private async Task LoadGymObjectIntoForm()
    {
        switch (_gymObjectTypeName)
        {
            case nameof(Bar):
                _gymObject = await _barRepo.LoadOneById(_gymObjectId);
                break;

            case nameof(Plate):
                _gymObject = await _plateRepo.LoadOneById(_gymObjectId);
                break;

            case nameof(Barbell):
                _gymObject = await _barbellRepo.LoadOneById(_gymObjectId);
                break;

            case nameof(Dumbbell):
                _gymObject = await _dumbbellRepo.LoadOneById(_gymObjectId);
                break;

            case nameof(Kettlebell):
                _gymObject = await _kettlebellRepo.LoadOneById(_gymObjectId);
                break;

            default:
                throw new NoMatchingCaseException(
                    $"Invalid gym object type '{_gymObjectTypeName}'.");
        }

        if (_gymObject == null)
        {
            throw new DataException(
                $"{_gymObjectTypeName} object with Id = {_gymObjectId} was not found.");
        }

        // Set the form values.
        WeightText = _gymObject.Weight.ToString(CultureInfo.InvariantCulture);
        Units = _gymObject.Units;
        Enabled = _gymObject.Enabled;

        // Set the type-specific form values.
        switch (_gymObjectTypeName)
        {
            case nameof(Plate):
                var plate = (Plate)_gymObject;
                MainColor = plate.Color;
                break;

            case nameof(Dumbbell):
                var dumbbell = (Dumbbell)_gymObject;
                MainColor = dumbbell.Color;
                break;

            case nameof(Kettlebell):
                var kettlebell = (Kettlebell)_gymObject;
                MainColor = kettlebell.BallColor;
                HasBands = kettlebell.HasBands;
                BandColor = kettlebell.BandColor;
                break;
        }
    }

    // ---------------------------------------------------------------------------------------------
    // Save-related methods.

    /// <summary>
    /// Copy common values from the viewmodel to the model.
    /// </summary>
    private void CopyCommonValues(GymObject gymObject)
    {
        gymObject.Weight = _weight;
        gymObject.Units = Units;
        gymObject.Enabled = Enabled;
    }

    private async Task<GymObject> SaveBar()
    {
        // If this is an add operation, create new Bar.
        var bar = (_operation == "add" || _gymObject == null) ? new Bar() : (Bar)_gymObject;

        // Copy values from the viewmodel to the model.
        CopyCommonValues(bar);

        // Update the database.
        await _barRepo.Upsert(bar);

        return bar;
    }

    private async Task<GymObject?> SavePlate()
    {
        // If this is an add operation, create new Bar.
        var plate = (_operation == "add" || _gymObject == null) ? new Plate() : (Plate)_gymObject;

        // Copy values from the viewmodel to the model.
        CopyCommonValues(plate);
        plate.Color = MainColor;

        // Update the database.
        await _plateRepo.Upsert(plate);

        return plate;
    }

    private async Task<GymObject?> SaveBarbell()
    {
        // If this is an add operation, create new Barbell.
        var barbell = (_operation == "add" || _gymObject == null)
            ? new Barbell()
            : (Barbell)_gymObject;

        // Copy values from the viewmodel to the model.
        CopyCommonValues(barbell);

        // Update the database.
        await _barbellRepo.Upsert(barbell);

        return barbell;
    }

    private async Task<GymObject?> SaveDumbbell()
    {
        // If this is an add operation, create new Dumbbell.
        var dumbbell = (_operation == "add" || _gymObject == null)
            ? new Dumbbell()
            : (Dumbbell)_gymObject;

        // Copy values from the viewmodel to the model.
        CopyCommonValues(dumbbell);
        dumbbell.Color = MainColor;

        // Update the database.
        await _dumbbellRepo.Upsert(dumbbell);

        return dumbbell;
    }

    private async Task<GymObject?> SaveKettlebell()
    {
        // If this is an add operation, create new Kettlebell.
        var kettlebell = (_operation == "add" || _gymObject == null)
            ? new Kettlebell()
            : (Kettlebell)_gymObject;

        // Copy values from the viewmodel to the model.
        CopyCommonValues(kettlebell);
        kettlebell.BallColor = MainColor;
        kettlebell.HasBands = HasBands;
        kettlebell.BandColor = BandColor;

        // Update the database.
        await _kettlebellRepo.Upsert(kettlebell);

        return kettlebell;
    }
}
