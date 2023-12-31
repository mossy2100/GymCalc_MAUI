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
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="barbellRepo"></param>
    /// <param name="dumbbellRepo"></param>
    /// <param name="kettlebellRepo"></param>
    public EditViewModel(BarRepository barRepo, PlateRepository plateRepo,
        BarbellRepository barbellRepo, DumbbellRepository dumbbellRepo,
        KettlebellRepository kettlebellRepo)
    {
        // Dependencies.
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _barbellRepo = barbellRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;

        // Commands.
        CancelCommand = new AsyncCommand(Cancel);
        SaveCommand = new AsyncCommand(Save);
    }

    #region Dependencies

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly BarbellRepository _barbellRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    #endregion Dependencies

    #region Fields

    private bool _enabled;

    private string? _errorMessage;

    private GymObject? _gymObject;

    private int _gymObjectId;

    private string? _gymObjectTypeName;

    private string? _color;

    private string? _operation;

    private string? _title;

    private string? _units;

    private bool _canHaveBands;

    private EBandsOption _bandsOption;

    private bool _bandsOptionNoneChecked;

    private bool _bandsOptionBlackChecked;

    private bool _bandsOptionColorChecked;

    /// <summary>
    /// The weight (parsed from the WeightText entry field).
    /// </summary>
    private decimal _weight;

    private string? _weightText;

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

    public string? Color
    {
        get => _color;

        set => SetProperty(ref _color, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;

        set => SetProperty(ref _errorMessage, value);
    }

    public bool CanHaveBands
    {
        get => _canHaveBands;

        set => SetProperty(ref _canHaveBands, value);
    }

    public EBandsOption BandsOption
    {
        get => _bandsOption;

        set
        {
            // Update the backing field.
            _bandsOption = value;

            // Set the checked flags for each radio.
            BandsOptionNoneChecked = value == EBandsOption.None;
            BandsOptionBlackChecked = value == EBandsOption.Black;
            BandsOptionColorChecked = value == EBandsOption.Color;
        }
    }

    public bool BandsOptionNoneChecked
    {
        get => _bandsOptionNoneChecked;

        set
        {
            if (value)
            {
                BandsOption = EBandsOption.None;
            }

            // Fire the OnPropertyChanged event.
            SetProperty(ref _bandsOptionNoneChecked, value);
        }
    }

    public bool BandsOptionBlackChecked
    {
        get => _bandsOptionBlackChecked;

        set
        {
            if (value)
            {
                BandsOption = EBandsOption.Black;
            }

            // Fire the OnPropertyChanged event.
            SetProperty(ref _bandsOptionBlackChecked, value);
        }
    }

    public bool BandsOptionColorChecked
    {
        get => _bandsOptionColorChecked;

        set
        {
            if (value)
            {
                BandsOption = EBandsOption.Color;
            }

            // Fire the OnPropertyChanged event.
            SetProperty(ref _bandsOptionColorChecked, value);
        }
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
        // Validate the form.
        bool weightOk = decimal.TryParse(WeightText, out decimal weight) && weight > 0;
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
            _ => throw new MatchNotFoundException("Invalid gym object type.")
        };

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
        Units = UnitsService.GetDefaultUnitsSymbol();
        Enabled = true;
        Color = "Black";
        CanHaveBands = false;
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

        // Load the object.
        _gymObject = _gymObjectTypeName switch
        {
            nameof(Bar) => await _barRepo.LoadById(_gymObjectId),
            nameof(Plate) => await _plateRepo.LoadById(_gymObjectId),
            nameof(Barbell) => await _barbellRepo.LoadById(_gymObjectId),
            nameof(Dumbbell) => await _dumbbellRepo.LoadById(_gymObjectId),
            nameof(Kettlebell) => await _kettlebellRepo.LoadById(_gymObjectId),
            _ => throw new MatchNotFoundException(
                $"Invalid gym object type '{_gymObjectTypeName}'.")
        };

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
            CanHaveBands = true;
            BandsOption = kettlebell.BandsOption;
        }
        else
        {
            CanHaveBands = false;
        }
    }

    /// <summary>
    /// Copy common values from the viewmodel to the model.
    /// </summary>
    private void CopyCommonValues(GymObject gymObject)
    {
        gymObject.Weight = _weight;
        gymObject.Units = Units;
        gymObject.Enabled = Enabled;
        gymObject.Color = Color;
    }

    #endregion Other methods

    #region Save methods

    private async Task<GymObject> SaveBar()
    {
        // If this is an add operation, create new Bar.
        Bar? bar = (_operation == "add" || _gymObject == null) ? new Bar() : (Bar)_gymObject;

        // Copy values from the viewmodel to the model.
        CopyCommonValues(bar);

        // Update the database.
        await _barRepo.Upsert(bar);

        return bar;
    }

    private async Task<GymObject?> SavePlate()
    {
        // If this is an add operation, create new Bar.
        Plate? plate = (_operation == "add" || _gymObject == null)
            ? new Plate()
            : (Plate)_gymObject;

        // Copy values from the viewmodel to the model.
        CopyCommonValues(plate);

        // Update the database.
        await _plateRepo.Upsert(plate);

        return plate;
    }

    private async Task<GymObject?> SaveBarbell()
    {
        // If this is an add operation, create new Barbell.
        Barbell? barbell = (_operation == "add" || _gymObject == null)
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
        Dumbbell? dumbbell = (_operation == "add" || _gymObject == null)
            ? new Dumbbell()
            : (Dumbbell)_gymObject;

        // Copy values from the viewmodel to the model.
        CopyCommonValues(dumbbell);

        // Update the database.
        await _dumbbellRepo.Upsert(dumbbell);

        return dumbbell;
    }

    private async Task<GymObject?> SaveKettlebell()
    {
        // If this is an add operation, create new Kettlebell.
        Kettlebell? kettlebell = (_operation == "add" || _gymObject == null)
            ? new Kettlebell()
            : (Kettlebell)_gymObject;

        // Copy values from the viewmodel to the model.
        CopyCommonValues(kettlebell);
        kettlebell.BandsOption = BandsOption;

        // Update the database.
        await _kettlebellRepo.Upsert(kettlebell);

        return kettlebell;
    }

    #endregion Save methods
}
