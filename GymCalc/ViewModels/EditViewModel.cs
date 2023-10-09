using System.Globalization;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Constants;
using GymCalc.Data;
using GymCalc.Models;

namespace GymCalc.ViewModels;

public class EditViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    // Dependencies.

    private readonly Database _database;

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly DumbbellRepository _dbRepo;

    private readonly KettlebellRepository _kbRepo;

    // ---------------------------------------------------------------------------------------------
    // Commands.

    public ICommand CancelCommand { get; init; }

    public ICommand SaveItemCommand { get; init; }

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
    /// The weight (obtained from the entry).
    /// </summary>
    private double _weight;

    // ---------------------------------------------------------------------------------------------
    // Bindable properties.

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

    // private bool _bandColorGridIsVisible;
    //
    // public bool BandColorGridIsVisible
    // {
    //     get => _bandColorGridIsVisible;
    //
    //     set => SetProperty(ref _bandColorGridIsVisible, value);
    // }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database"></param>
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="dbRepo"></param>
    /// <param name="kbRepo"></param>
    public EditViewModel(Database database, BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dbRepo, KettlebellRepository kbRepo)
    {
        // Dependencies.
        _database = database;
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dbRepo = dbRepo;
        _kbRepo = kbRepo;

        // Commands.
        CancelCommand = new AsyncCommand(Cancel);
        SaveItemCommand = new AsyncCommand(SaveItem);
    }

    // ---------------------------------------------------------------------------------------------
    // Command methods.

    /// <summary>
    /// Go back to the list page, showing items of the current type.
    /// </summary>
    private async Task Cancel()
    {
        // await AppShell.GoToList(_gymObjectTypeName);
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Save the item to the database, then go back to the list page.
    /// </summary>
    private async Task SaveItem()
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

        // Update the object. This will vary by type.
        _gymObject = _gymObjectTypeName switch
        {
            GymObjectType.Bar => UpdateBar(),
            GymObjectType.Plate => UpdatePlate(),
            GymObjectType.Dumbbell => UpdateDumbbell(),
            GymObjectType.Kettlebell => UpdateKettlebell(),
            _ => throw new Exception("Invalid gym object type.")
        };

        // Copy common values from the view model to the model.
        _gymObject.Weight = _weight;
        _gymObject.Units = Units;
        _gymObject.Enabled = Enabled;

        // Save the object to the database.
        var conn = _database.Connection;
        if (_operation == "add")
        {
            await conn.InsertAsync(_gymObject);
        }
        else
        {
            await conn.UpdateAsync(_gymObject);
        }

        // Go back to the list of this object type.
        await AppShell.GoToList(_gymObjectTypeName);
    }

    /// <summary>
    /// Hide and show the form fields appropriate to this object type.
    /// </summary>
    internal async Task<bool> Initialize(string operation, string gymObjectTypeName,
        int gymObjectId)
    {
        // Check if all the necessary parameters have been set.
        bool ready = string.IsNullOrEmpty(gymObjectTypeName)
            && ((operation == "edit" && gymObjectId > 0) || operation == "add");
        if (!ready)
        {
            return false;
        }

        // Remember the view parameters.
        _operation = operation;
        _gymObjectTypeName = gymObjectTypeName;
        _gymObjectId = gymObjectId;

        // Hide certain fields according to the object type, as if creating a new object.
        switch (gymObjectTypeName)
        {
            case GymObjectType.Bar:
                MainColorIsVisible = false;
                HasBandsIsVisible = false;
                break;

            case GymObjectType.Plate:
            case GymObjectType.Dumbbell:
                MainColorIsVisible = true;
                HasBandsIsVisible = false;
                break;

            case GymObjectType.Kettlebell:
                MainColorIsVisible = true;
                HasBandsIsVisible = true;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(gymObjectTypeName), "Invalid object type.");
        }

        // If editing an existing object, load it from the database and populate the form.
        if (operation == "edit")
        {
            await LoadItem();
        }

        return true;
    }

    private async Task LoadItem()
    {
        switch (_gymObjectTypeName)
        {
            case GymObjectType.Bar:
                Bar bar = await _barRepo.Get(_gymObjectId);
                _gymObject = bar;
                break;

            case GymObjectType.Plate:
                Plate plate = await _plateRepo.Get(_gymObjectId);
                MainColor = plate.Color;
                _gymObject = plate;
                break;

            case GymObjectType.Dumbbell:
                Dumbbell dumbbell = await _dbRepo.Get(_gymObjectId);
                MainColor = dumbbell.Color;
                _gymObject = dumbbell;
                break;

            case GymObjectType.Kettlebell:
                Kettlebell kettlebell = await _kbRepo.Get(_gymObjectId);
                MainColor = kettlebell.BallColor;
                HasBands = kettlebell.HasBands;
                BandColor = kettlebell.BandColor;
                _gymObject = kettlebell;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(_gymObjectId), "Invalid object id.");
        }

        // Set the common form values.
        WeightText = _gymObject.Weight.ToString(CultureInfo.InvariantCulture);
        Units = _gymObject.Units;
        Enabled = _gymObject.Enabled;
    }

    // ---------------------------------------------------------------------------------------------
    // Update methods for each type.

    private GymObject UpdateBar()
    {
        // If this is an add operation, create new Bar.
        Bar bar = _operation == "add" ? new Bar() : (Bar)_gymObject;

        return bar;
    }

    private GymObject UpdatePlate()
    {
        // If this is an add operation, create new Bar.
        Plate plate = _operation == "add" ? new Plate() : (Plate)_gymObject;

        // Copy values from the view model to the model.
        plate.Color = MainColor;

        return plate;
    }

    private GymObject UpdateDumbbell()
    {
        // If this is an add operation, create new Dumbbell.
        Dumbbell dumbbell = _operation == "add" ? new Dumbbell() : (Dumbbell)_gymObject;

        // Copy values from the view model to the model.
        dumbbell.Color = MainColor;

        return dumbbell;
    }

    private GymObject UpdateKettlebell()
    {
        // If this is an add operation, create new Kettlebell.
        Kettlebell kettlebell = _operation == "add" ? new Kettlebell() : (Kettlebell)_gymObject;

        // Copy values from the view model to the model.
        kettlebell.BallColor = MainColor;
        kettlebell.HasBands = HasBands;
        kettlebell.BandColor = BandColor;

        return kettlebell;
    }
}
