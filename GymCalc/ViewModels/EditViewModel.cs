using System.Globalization;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using Galaxon.Core.Enums;
using Galaxon.Core.Exceptions;
using GymCalc.Data;
using GymCalc.Models;
using GymCalc.Utilities;

namespace GymCalc.ViewModels;

public class EditViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    // Dependencies.

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    // ---------------------------------------------------------------------------------------------
    // Commands.

    public ICommand CancelCommand { get; init; }

    public ICommand SaveCommand { get; init; }

    // ---------------------------------------------------------------------------------------------
    // Page parameters.

    private string _operation;

    private string _gymObjectTypeName;

    private int _gymObjectId;

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// The gym object.
    /// </summary>
    private GymObject _gymObject;

    /// <summary>
    /// The weight (parsed from the WeightText entry field).
    /// </summary>
    private double _weight;

    // ---------------------------------------------------------------------------------------------
    // Bindable properties.

    private string _title;

    public string Title
    {
        get => _title;

        set => SetProperty(ref _title, value);
    }

    private string _weightText;

    public string WeightText
    {
        get => _weightText;

        set => SetProperty(ref _weightText, value);
    }

    private string _units;

    public string Units
    {
        get => _units;

        set => SetProperty(ref _units, value);
    }

    private bool _enabled;

    public bool Enabled
    {
        get => _enabled;

        set => SetProperty(ref _enabled, value);
    }

    private string _mainColor;

    public string MainColor
    {
        get => _mainColor;

        set => SetProperty(ref _mainColor, value);
    }

    private bool _hasBands;

    public bool HasBands
    {
        get => _hasBands;

        set => SetProperty(ref _hasBands, value);
    }

    private string _bandColor;

    public string BandColor
    {
        get => _bandColor;

        set => SetProperty(ref _bandColor, value);
    }

    private string _errorMessage;

    public string ErrorMessage
    {
        get => _errorMessage;

        set => SetProperty(ref _errorMessage, value);
    }

    private bool _mainColorIsVisible;

    public bool MainColorIsVisible
    {
        get => _mainColorIsVisible;

        set => SetProperty(ref _mainColorIsVisible, value);
    }

    private bool _hasBandsIsVisible;

    public bool HasBandsIsVisible
    {
        get => _hasBandsIsVisible;

        set => SetProperty(ref _hasBandsIsVisible, value);
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="dumbbellRepo"></param>
    /// <param name="kettlebellRepo"></param>
    public EditViewModel(BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dumbbellRepo, KettlebellRepository kettlebellRepo)
    {
        // Dependencies.
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;

        // Commands.
        CancelCommand = new AsyncCommand(Cancel);
        SaveCommand = new AsyncCommand(SaveGymObject);
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
        var weightOk = double.TryParse(WeightText, out var weight) && weight > 0;
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
    internal async Task Initialize(string operation, string gymObjectTypeName,
        int gymObjectId)
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
            await LoadGymObject();
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
    /// <exception cref="NoMatchingCaseException"></exception>
    private async Task LoadGymObject()
    {
        switch (_gymObjectTypeName)
        {
            case nameof(Bar):
                Bar bar = await _barRepo.Get(_gymObjectId);
                _gymObject = bar;
                break;

            case nameof(Plate):
                Plate plate = await _plateRepo.Get(_gymObjectId);
                MainColor = plate.Color;
                _gymObject = plate;
                break;

            case nameof(Dumbbell):
                Dumbbell dumbbell = await _dumbbellRepo.Get(_gymObjectId);
                MainColor = dumbbell.Color;
                _gymObject = dumbbell;
                break;

            case nameof(Kettlebell):
                Kettlebell kettlebell = await _kettlebellRepo.Get(_gymObjectId);
                MainColor = kettlebell.BallColor;
                HasBands = kettlebell.HasBands;
                BandColor = kettlebell.BandColor;
                _gymObject = kettlebell;
                break;

            default:
                throw new NoMatchingCaseException(
                    $"Invalid gym object type '{_gymObjectTypeName}'.");
        }

        // Set the common form values.
        WeightText = _gymObject.Weight.ToString(CultureInfo.InvariantCulture);
        Units = _gymObject.Units;
        Enabled = _gymObject.Enabled;
    }

    // ---------------------------------------------------------------------------------------------
    // Save-related methods.

    /// <summary>
    /// Copy common values from the view model to the model.
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
        Bar bar = _operation == "add" ? new Bar() : (Bar)_gymObject;

        // Copy values from the view model to the model.
        CopyCommonValues(bar);

        // Update the database.
        await _barRepo.Upsert(bar);

        return bar;
    }

    private async Task<GymObject> SavePlate()
    {
        // If this is an add operation, create new Bar.
        Plate plate = _operation == "add" ? new Plate() : (Plate)_gymObject;

        // Copy values from the view model to the model.
        CopyCommonValues(plate);
        plate.Color = MainColor;

        // Update the database.
        await _plateRepo.Upsert(plate);

        return plate;
    }

    private async Task<GymObject> SaveDumbbell()
    {
        // If this is an add operation, create new Dumbbell.
        Dumbbell dumbbell = _operation == "add" ? new Dumbbell() : (Dumbbell)_gymObject;

        // Copy values from the view model to the model.
        CopyCommonValues(dumbbell);
        dumbbell.Color = MainColor;

        // Update the database.
        await _dumbbellRepo.Upsert(dumbbell);

        return dumbbell;
    }

    private async Task<GymObject> SaveKettlebell()
    {
        // If this is an add operation, create new Kettlebell.
        Kettlebell kettlebell = _operation == "add" ? new Kettlebell() : (Kettlebell)_gymObject;

        // Copy values from the view model to the model.
        CopyCommonValues(kettlebell);
        kettlebell.BallColor = MainColor;
        kettlebell.HasBands = HasBands;
        kettlebell.BandColor = BandColor;

        // Update the database.
        await _kettlebellRepo.Upsert(kettlebell);

        return kettlebell;
    }
}
