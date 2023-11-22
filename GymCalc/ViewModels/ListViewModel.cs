using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using Galaxon.Core.Types;
using GymCalc.Data;
using GymCalc.Drawables;
using GymCalc.Models;
using GymCalc.Shared;

namespace GymCalc.ViewModels;

public class ListViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    // Dependencies.

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly BarbellRepository _barbellRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    // ---------------------------------------------------------------------------------------------
    // Bindable properties.

    /// <summary>
    /// The type of gym objects listed. This is set by the page, which receives it as a parameter.
    /// </summary>
    private string _gymObjectTypeName;

    /// <summary>
    /// Instructions text.
    /// </summary>
    private string _instructions;

    /// <summary>
    /// Results for the CollectionView.
    /// </summary>
    private List<ListItem> _listItems;

    /// <summary>
    /// Page title.
    /// </summary>
    private string _title;

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="barbellRepo"></param>
    /// <param name="dumbbellRepo"></param>
    /// <param name="kettlebellRepo"></param>
    public ListViewModel(BarRepository barRepo, PlateRepository plateRepo,
        BarbellRepository barbellRepo, DumbbellRepository dumbbellRepo,
        KettlebellRepository kettlebellRepo)
    {
        // Keep references to the dependencies.
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

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set => SetProperty(ref _gymObjectTypeName, value);
    }

    public List<ListItem> ListItems
    {
        get => _listItems;

        set => SetProperty(ref _listItems, value);
    }

    public string Title
    {
        get => _title;

        set => SetProperty(ref _title, value);
    }

    public string Instructions
    {
        get => _instructions;

        set => SetProperty(ref _instructions, value);
    }

    // ---------------------------------------------------------------------------------------------
    // Commands.

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

    // ---------------------------------------------------------------------------------------------
    internal async Task DisplayList()
    {
        // Make sure GymObjectTypeName is set.
        if (string.IsNullOrWhiteSpace(GymObjectTypeName))
        {
            return;
        }

        Title = $"{GymObjectTypeName}s";

        Instructions = $"Use the checkboxes to select which {GymObjectTypeName.ToLower()}"
            + $" weights ({UnitsUtility.GetDefault().GetDescription()}) are available."
            + $" Use the edit and delete icon buttons to make changes."
            + $" Use the Add button to add a new {GymObjectTypeName.ToLower()}, or the Reset"
            + $" button to reset to the defaults.";

        // Display all gym objects of the specified type.
        switch (GymObjectTypeName)
        {
            case nameof(Bar):
                var bars = await _barRepo.GetSome(ascending: true);
                DisplayList(bars);
                break;

            case nameof(Plate):
                var plates = await _plateRepo.GetSome(ascending: true);
                DisplayList(plates);
                break;

            case nameof(Barbell):
                var barbells = await _barbellRepo.GetSome(ascending: true);
                DisplayList(barbells);
                break;

            case nameof(Dumbbell):
                var dumbbells = await _dumbbellRepo.GetSome(ascending: true);
                DisplayList(dumbbells);
                break;

            case nameof(Kettlebell):
                var kettlebells = await _kettlebellRepo.GetSome(ascending: true);
                DisplayList(kettlebells);
                break;
        }
    }

    /// <summary>
    /// Initialize the list of gym objects.
    /// </summary>
    private void DisplayList<T>(List<T> gymObjects) where T : GymObject
    {
        // Initialize the empty list.
        ListItems = new List<ListItem>();

        // Check if there's anything to draw.
        if (gymObjects.Count == 0)
        {
            return;
        }

        // Get the maximum weight, which is used to determine the width of bars and plates.
        var maxWeight = gymObjects.Last().Weight;

        // Create drawables and add to list.
        foreach (var gymObject in gymObjects)
        {
            // Create the drawable.
            var drawable = GymObjectDrawable.Create(gymObject);
            drawable.MaxWeight = maxWeight;

            // Create the list item and add it to the list.
            var listItem = new ListItem
            {
                GymObject = gymObject,
                Drawable = drawable,
                Enabled = gymObject.Enabled
            };
            ListItems.Add(listItem);
        }
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(GymObjectTypeName):
                Task.Run(async () => await DisplayList()).Wait();
                break;
        }
    }

    // ---------------------------------------------------------------------------------------------
    // Command methods.

    private async Task EnableGymObject(ListItem listItem)
    {
        // Check if an update is actually needed. This method is called on initialization of the
        // CollectionView, when no changes have occurred. Could be a bug in InputKit.
        if (listItem.Enabled == listItem.GymObject.Enabled)
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

    private async Task EditGymObject(GymObject gymObject)
    {
        await Shell.Current.GoToAsync($"edit?op=edit&type={GymObjectTypeName}&id={gymObject.Id}");
    }

    private async Task DeleteGymObject(GymObject gymObject)
    {
        await Shell.Current.GoToAsync($"delete?type={GymObjectTypeName}&id={gymObject.Id}");
    }

    private async Task AddGymObject()
    {
        await Shell.Current.GoToAsync($"edit?op=add&type={GymObjectTypeName}");
    }

    private async Task ResetGymObjects()
    {
        await Shell.Current.GoToAsync($"reset?type={GymObjectTypeName}");
    }
}
