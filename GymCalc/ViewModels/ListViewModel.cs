using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using Galaxon.Core.Enums;
using GymCalc.Constants;
using GymCalc.Data;
using GymCalc.Drawables;
using GymCalc.Models;
using GymCalc.Utilities;

namespace GymCalc.ViewModels;

public class ListViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    // Dependencies.

    private readonly Database _database;

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly DumbbellRepository _dbRepo;

    private readonly KettlebellRepository _kbRepo;

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// The type of gym objects listed. This is set from the page, which receives it as a parameter.
    /// </summary>
    private string _gymObjectTypeName;

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set => SetProperty(ref _gymObjectTypeName, value);
    }

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Results for the CollectionView.
    /// </summary>
    private List<GymObjectDrawable> _drawables;

    public List<GymObjectDrawable> Drawables
    {
        get => _drawables;

        set => SetProperty(ref _drawables, value);
    }

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Page title.
    /// </summary>
    private string _title;

    public string Title
    {
        get => _title;

        set => SetProperty(ref _title, value);
    }

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Instructions text.
    /// </summary>
    private string _instructionsText;

    public string InstructionsText
    {
        get => _instructionsText;

        set => SetProperty(ref _instructionsText, value);
    }

    // ---------------------------------------------------------------------------------------------
    // Commands.

    /// <summary>
    /// Command to enable/disable an item.
    /// </summary>
    public ICommand EnableItemCommand { get; init; }

    /// <summary>
    /// Command to edit an item.
    /// </summary>
    public ICommand EditItemCommand { get; init; }

    /// <summary>
    /// Command to delete an item.
    /// </summary>
    public ICommand DeleteItemCommand { get; init; }

    /// <summary>
    /// Command to add a new item.
    /// </summary>
    public ICommand AddItemCommand { get; init; }

    /// <summary>
    /// Reset items command.
    /// </summary>
    public ICommand ResetItemsCommand { get; init; }

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database"></param>
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="dbRepo"></param>
    /// <param name="kbRepo"></param>
    public ListViewModel(Database database, BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dbRepo, KettlebellRepository kbRepo)
    {
        // Keep references to the dependencies.
        _database = database;
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dbRepo = dbRepo;
        _kbRepo = kbRepo;

        // Commands.
        EnableItemCommand = new AsyncCommand<GymObject>(EnableItem);
        EditItemCommand = new AsyncCommand<GymObject>(EditItem);
        DeleteItemCommand = new AsyncCommand<GymObject>(DeleteItem);
        AddItemCommand = new AsyncCommand(AddItem);
        ResetItemsCommand = new AsyncCommand(ResetItems);
    }

    // ---------------------------------------------------------------------------------------------
    internal async Task DisplayList()
    {
        // Make sure GymObjectTypeName is set.
        if (string.IsNullOrWhiteSpace(GymObjectTypeName))
        {
            return;
        }

        Title = $"{GymObjectTypeName}s";

        InstructionsText = $"Use the checkboxes to select which {GymObjectTypeName.ToLower()}"
            + $" weights ({UnitsUtility.GetDefault().GetDescription()}) are available."
            + $" Use the edit and delete icon buttons to make changes."
            + $" Use the Add button to add a new {GymObjectTypeName.ToLower()}, or the Reset"
            + $" button to reset to the defaults.";

        // Display all gym objects of the specified type.
        switch (GymObjectTypeName)
        {
            case GymObjectType.Bar:
                var bars = await _barRepo.GetAll(ascending: true);
                DisplayList<Bar, BarDrawable>(bars);
                break;

            case GymObjectType.Plate:
                var plates = await _plateRepo.GetAll(ascending: true);
                DisplayList<Plate, PlateDrawable>(plates);
                break;

            case GymObjectType.Dumbbell:
                var dumbbells = await _dbRepo.GetAll(ascending: true);
                DisplayList<Dumbbell, DumbbellDrawable>(dumbbells);
                break;

            case GymObjectType.Kettlebell:
                var kettlebells = await _kbRepo.GetAll(ascending: true);
                DisplayList<Kettlebell, KettlebellDrawable>(kettlebells);
                break;
        }
    }

    /// <summary>
    /// Initialize the list of gym objects.
    /// </summary>
    private void DisplayList<T, TDrawable>(List<T> gymObjects)
        where T : GymObject
        where TDrawable : GymObjectDrawable, new()
    {
        // Initialize the empty list.
        Drawables = new List<GymObjectDrawable>();

        // Get the maximum weight, which is used to determine the width of bars and plates.
        var maxWeight = gymObjects.Last().Weight;

        // Construct drawables and add to list.
        foreach (var gymObject in gymObjects)
        {
            var drawable = new TDrawable
            {
                GymObject = gymObject,
                MaxWeight = maxWeight,
            };
            Drawables.Add(drawable);
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(GymObjectTypeName):
                Task.Run(DisplayList).Wait();
                break;
        }
    }

    // ---------------------------------------------------------------------------------------------
    // Command methods.

    private async Task EnableItem(GymObject gymObject)
    {
        await _database.Connection.UpdateAsync(gymObject);
    }

    private async Task EditItem(GymObject gymObject)
    {
        await Shell.Current.GoToAsync($"edit?op=edit&type={GymObjectTypeName}&id={gymObject.Id}");
    }

    private async Task DeleteItem(GymObject gymObject)
    {
        await Shell.Current.GoToAsync($"delete?type={GymObjectTypeName}&id={gymObject.Id}");
    }

    private async Task AddItem()
    {
        await Shell.Current.GoToAsync($"edit?op=add&type={GymObjectTypeName}");
    }

    private async Task ResetItems()
    {
        await Shell.Current.GoToAsync($"reset?type={GymObjectTypeName}");
    }
}
