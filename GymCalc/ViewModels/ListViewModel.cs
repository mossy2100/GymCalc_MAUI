using AsyncAwaitBestPractices.MVVM;
using GymCalc.Drawables;
using GymCalc.Models;
using GymCalc.Repositories;
using GymCalc.Services;

namespace GymCalc.ViewModels;

public class ListViewModel : BaseViewModel
{
    #region Dependencies

    private readonly Database _database;

    private IGymObjectRepository? _repo;

    #endregion Dependencies

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database"></param>
    public ListViewModel(Database database)
    {
        // Keep references to dependencies.
        _database = database;

        // Commands.
        EnableCommand = new AsyncCommand<ListItem>(EnableGymObject);
        EditCommand = new AsyncCommand<GymObject>(EditGymObject);
        DeleteCommand = new AsyncCommand<GymObject>(DeleteGymObject);
        AddCommand = new AsyncCommand(AddGymObject);
        ResetCommand = new AsyncCommand(ResetGymObjects);
    }

    #endregion Constructor

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
        // Guard.
        if (_repo == null)
        {
            throw new InvalidOperationException("Reference to repository not set.");
        }

        // Check if an update is actually needed. This method is called on initialization of the
        // CollectionView, when no changes have occurred. Could be a bug in InputKit.
        if (listItem!.Enabled == listItem.GymObject.Enabled)
        {
            return;
        }

        // Update the object in the database.
        listItem.GymObject.Enabled = listItem.Enabled;
        await _repo.Upsert(listItem.GymObject);
    }

    /// <summary>
    /// Go to the form page for editing a gym object.
    /// </summary>
    /// <param name="gymObject"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task EditGymObject(GymObject? gymObject)
    {
        // Guard.
        if (gymObject == null)
        {
            throw new InvalidOperationException("Gym object not set.");
        }

        await Shell.Current.GoToAsync($"edit?op=edit&type={_gymObjectTypeName}&id={gymObject.Id}");
    }

    /// <summary>
    /// Delete a gym object and refresh the list.
    /// </summary>
    /// <param name="gymObject">The gym object to delete.</param>
    /// <exception cref="InvalidOperationException"></exception>
    internal async Task DeleteGymObject(GymObject? gymObject)
    {
        // Guards.
        if (gymObject == null)
        {
            throw new InvalidOperationException("Gym object not set.");
        }
        if (_repo == null)
        {
            throw new InvalidOperationException("Reference to repository not set.");
        }

        // Delete the object from the database.
        await _repo.Delete(gymObject);

        // Refresh the list of objects.
        await DisplayList();
    }

    /// <summary>
    /// Go to the form page for adding a new gym object.
    /// </summary>
    private async Task AddGymObject()
    {
        await Shell.Current.GoToAsync($"edit?op=add&type={_gymObjectTypeName}");
    }

    /// <summary>
    /// Reset the gym objects to the default.
    /// </summary>
    internal async Task ResetGymObjects()
    {
        // Guard.
        if (_repo == null)
        {
            throw new InvalidOperationException("Reference to repository not set.");
        }

        // Delete all current objects of the specified type.
        await _repo.DeleteAll();

        // Insert the defaults.
        await _repo.InsertDefaults();

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

    #region UI methods

    /// <summary>
    /// Display the list of objects on the page.
    /// </summary>
    private async Task DisplayList()
    {
        // Make sure GymObjectTypeName is set.
        if (string.IsNullOrWhiteSpace(_gymObjectTypeName))
        {
            return;
        }

        // Set the title and instructions.
        Title = $"{_gymObjectTypeName}s";
        Instructions = $"Use the checkboxes to select which {_gymObjectTypeName.ToLower()}"
            + $" weights ({UnitsService.GetDefaultUnitsSymbol()}) are available."
            + $" Use the edit and delete icon buttons to make changes."
            + $" Use the Add button to add a new {_gymObjectTypeName.ToLower()}, or the Reset"
            + $" button to reset to the defaults.";

        // Get all gym objects of the specified type.
        _repo = _database.GetRepo(_gymObjectTypeName);
        List<GymObject> gymObjects = await _repo.LoadAll();

        // Initialize the list of items.
        ListItems = new List<ListItem>();

        // Check if there's anything to draw.
        if (gymObjects.Count == 0)
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

    #endregion UI methods
}
