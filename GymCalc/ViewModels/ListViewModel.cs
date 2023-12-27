using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Drawables;
using GymCalc.Models;
using GymCalc.Repositories;
using GymCalc.Services;

namespace GymCalc.ViewModels;

public class ListViewModel : BaseViewModel
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database"></param>
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="barbellRepo"></param>
    /// <param name="dumbbellRepo"></param>
    /// <param name="kettlebellRepo"></param>
    public ListViewModel(Database database, BarRepository barRepo, PlateRepository plateRepo,
        BarbellRepository barbellRepo, DumbbellRepository dumbbellRepo,
        KettlebellRepository kettlebellRepo)
    {
        // Keep references to the dependencies.
        _database = database;
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _barbellRepo = barbellRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;

        // Commands.
        EnableCommand = new AsyncCommand<ListItem>(EnableGymObject);
        EditCommand = new AsyncCommand<GymObject>(EditGymObject);
        DeleteCommand = new AsyncCommand<GymObject>(DeleteGymObject);
        AddCommand = new AsyncCommand(AddGymObject);
        ResetCommand = new AsyncCommand(ResetGymObjects);
    }

    /// <summary>
    /// Display the list of objects.
    /// </summary>
    internal async Task DisplayList()
    {
        // Make sure GymObjectTypeName is set.
        if (string.IsNullOrWhiteSpace(GymObjectTypeName))
        {
            return;
        }

        Title = $"{GymObjectTypeName}s";

        Instructions = $"Use the checkboxes to select which {GymObjectTypeName.ToLower()}"
            + $" weights ({UnitsService.GetDefaultUnitsSymbol()}) are available."
            + $" Use the edit and delete icon buttons to make changes."
            + $" Use the Add button to add a new {GymObjectTypeName.ToLower()}, or the Reset"
            + $" button to reset to the defaults.";

        // Get all gym objects of the specified type.
        List<GymObject>? gymObjects = null;
        switch (GymObjectTypeName)
        {
            case nameof(Bar):
                List<Bar> bars = await _barRepo.LoadSome(null);
                gymObjects = bars.Cast<GymObject>().ToList();
                break;

            case nameof(Plate):
                List<Plate> plates = await _plateRepo.LoadSome(null);
                gymObjects = plates.Cast<GymObject>().ToList();
                break;

            case nameof(Barbell):
                List<Barbell> barbells = await _barbellRepo.LoadSome(null);
                gymObjects = barbells.Cast<GymObject>().ToList();
                break;

            case nameof(Dumbbell):
                List<Dumbbell> dumbbells = await _dumbbellRepo.LoadSome(null);
                gymObjects = dumbbells.Cast<GymObject>().ToList();
                break;

            case nameof(Kettlebell):
                List<Kettlebell> kettlebells = await _kettlebellRepo.LoadSome(null);
                gymObjects = kettlebells.Cast<GymObject>().ToList();
                break;
        }

        // Initialize the list of items.
        ListItems = new List<ListItem>();

        // Check if there's anything to draw.
        if (gymObjects == null || gymObjects.Count == 0)
        {
            return;
        }

        // Get the maximum weight, which is used to determine the width of bars and plates.
        decimal maxWeight = gymObjects.Last().Weight;

        // Create drawables and add to list.
        foreach (GymObject gymObject in gymObjects)
        {
            // Create the drawable.
            var drawable = GymObjectDrawable.Create(gymObject);
            drawable.MaxWeight = maxWeight;

            // Create the list item and add it to the list.
            var listItem = new ListItem(gymObject, drawable, gymObject.Enabled);
            ListItems.Add(listItem);
        }
    }

    #region Dependencies

    private readonly Database _database;

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly BarbellRepository _barbellRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    #endregion Dependencies

    #region Bindable properties

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// The type of gym objects listed. This is set by the page, which receives it as a parameter.
    /// </summary>
    private string? _gymObjectTypeName;

    public string? GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set => SetProperty(ref _gymObjectTypeName, value);
    }

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Results for the CollectionView.
    /// </summary>
    private List<ListItem>? _listItems;

    public List<ListItem>? ListItems
    {
        get => _listItems;

        set => SetProperty(ref _listItems, value);
    }

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Page title.
    /// </summary>
    private string? _title;

    public string? Title
    {
        get => _title;

        set => SetProperty(ref _title, value);
    }

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Instructions text.
    /// </summary>
    private string? _instructions;

    public string? Instructions
    {
        get => _instructions;

        set => SetProperty(ref _instructions, value);
    }

    #endregion Bindable properties

    #region Commands

    /// <summary>
    /// Command to enable/disable an item.
    /// </summary>
    public ICommand EnableCommand { get; init; }

    /// <summary>
    /// Command to edit an item.
    /// </summary>
    public ICommand EditCommand { get; init; }

    /// <summary>
    /// Command to delete an item.
    /// </summary>
    public ICommand DeleteCommand { get; init; }

    /// <summary>
    /// Command to add a new item.
    /// </summary>
    public ICommand AddCommand { get; init; }

    /// <summary>
    /// Reset items command.
    /// </summary>
    public ICommand ResetCommand { get; init; }

    #endregion Commands

    #region Command methods

    private async Task EnableGymObject(ListItem? listItem)
    {
        // Check if an update is actually needed. This method is called on initialization of the
        // CollectionView, when no changes have occurred. Could be a bug in InputKit.
        if (listItem!.Enabled == listItem.GymObject.Enabled)
        {
            return;
        }

        // Update the object in the database.
        listItem.GymObject.Enabled = listItem.Enabled;
        switch (listItem.GymObject)
        {
            case Bar bar:
                await _barRepo.Upsert(bar);
                break;

            case Plate plate:
                await _plateRepo.Upsert(plate);
                break;

            case Barbell barbell:
                await _barbellRepo.Upsert(barbell);
                break;

            case Dumbbell dumbbell:
                await _dumbbellRepo.Upsert(dumbbell);
                break;

            case Kettlebell kettlebell:
                await _kettlebellRepo.Upsert(kettlebell);
                break;
        }
    }

    private async Task EditGymObject(GymObject? gymObject)
    {
        if (gymObject == null)
        {
            throw new InvalidOperationException("Gym object not set.");
        }

        await Shell.Current.GoToAsync($"edit?op=edit&type={GymObjectTypeName}&id={gymObject.Id}");
    }

    /// <summary>
    /// Delete a gym object and refresh the list.
    /// </summary>
    /// <param name="gymObject">The gym object to delete.</param>
    /// <exception cref="InvalidOperationException"></exception>
    internal async Task DeleteGymObject(GymObject? gymObject)
    {
        if (gymObject == null)
        {
            throw new InvalidOperationException("Gym object not set.");
        }

        switch (gymObject)
        {
            case Bar:
                await _barRepo.Delete(gymObject.Id);
                break;

            case Plate:
                await _plateRepo.Delete(gymObject.Id);
                break;

            case Barbell:
                await _barbellRepo.Delete(gymObject.Id);
                break;

            case Dumbbell:
                await _dumbbellRepo.Delete(gymObject.Id);
                break;

            case Kettlebell:
                await _kettlebellRepo.Delete(gymObject.Id);
                break;
        }

        // Refresh the list of objects.
        await DisplayList();
    }

    private async Task AddGymObject()
    {
        await Shell.Current.GoToAsync($"edit?op=add&type={GymObjectTypeName}");
    }

    internal async Task ResetGymObjects()
    {
        // Get the repo.
        IGymObjectRepository repo = _database.GetRepo(GymObjectTypeName);

        // Delete all current objects of the specified type.
        await repo.DeleteAll();

        // Insert the defaults.
        await repo.InsertDefaults();

        // Refresh the list.
        await DisplayList();
    }

    #endregion Command methods

    #region Events

    /// <inheritdoc/>
    protected override async void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(GymObjectTypeName):
                await DisplayList();
                break;
        }
    }

    #endregion Events
}
