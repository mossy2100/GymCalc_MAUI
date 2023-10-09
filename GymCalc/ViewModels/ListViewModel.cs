using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using Galaxon.Core.Enums;
using Galaxon.Core.Exceptions;
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

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// The type of gym objects listed. This is set by the page, which receives it as a parameter.
    /// </summary>
    public string GymObjectTypeName { get; set; }

    // ---------------------------------------------------------------------------------------------
    // Bindable properties.

    /// <summary>
    /// Results for the CollectionView.
    /// </summary>
    private List<GymObjectDrawable> _drawables;

    public List<GymObjectDrawable> Drawables
    {
        get => _drawables;

        set => SetProperty(ref _drawables, value);
    }

    /// <summary>
    /// Page title.
    /// </summary>
    private string _title;

    public string Title
    {
        get => _title;

        set => SetProperty(ref _title, value);
    }

    /// <summary>
    /// Instructions text.
    /// </summary>
    private string _instructions;

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
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database"></param>
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="dumbbellRepo"></param>
    /// <param name="kettlebellRepo"></param>
    public ListViewModel(Database database, BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dumbbellRepo, KettlebellRepository kettlebellRepo)
    {
        // Keep references to the dependencies.
        _database = database;
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;

        // Commands.
        EnableCommand = new AsyncCommand<GymObject>(EnableItem);
        EditCommand = new AsyncCommand<GymObject>(EditItem);
        DeleteCommand = new AsyncCommand<GymObject>(DeleteItem);
        AddCommand = new AsyncCommand(AddItem);
        ResetCommand = new AsyncCommand(ResetItems);
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

        Instructions = $"Use the checkboxes to select which {GymObjectTypeName.ToLower()}"
            + $" weights ({UnitsUtility.GetDefault().GetDescription()}) are available."
            + $" Use the edit and delete icon buttons to make changes."
            + $" Use the Add button to add a new {GymObjectTypeName.ToLower()}, or the Reset"
            + $" button to reset to the defaults.";

        // Get the gym object type.
        if (!Enum.TryParse<GymObjectType>(GymObjectTypeName, out var gymObjectType))
        {
            throw new ArgumentInvalidException($"Invalid gym object type name ({GymObjectTypeName}).");
        }

        // Display all gym objects of the specified type.
        switch (gymObjectType)
        {
            case GymObjectType.Bar:
                var bars = await _barRepo.GetSome(ascending: true);
                DisplayList<Bar, BarDrawable>(bars);
                break;

            case GymObjectType.Plate:
                var plates = await _plateRepo.GetSome(ascending: true);
                DisplayList<Plate, PlateDrawable>(plates);
                break;

            case GymObjectType.Dumbbell:
                var dumbbells = await _dumbbellRepo.GetSome(ascending: true);
                DisplayList<Dumbbell, DumbbellDrawable>(dumbbells);
                break;

            case GymObjectType.Kettlebell:
                var kettlebells = await _kettlebellRepo.GetSome(ascending: true);
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

        // Check if there's anything to draw.
        if (gymObjects.Count == 0)
        {
            return;
        }

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
