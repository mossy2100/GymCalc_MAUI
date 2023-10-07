using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Constants;
using GymCalc.Data;
using GymCalc.Drawables;
using GymCalc.Models;

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
    /// If we are in edit mode or not.
    /// </summary>
    private bool _editMode;

    public bool EditMode
    {
        get => _editMode;

        set => SetProperty(ref _editMode, value);
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
    // Commands.

    /// <summary>
    /// Command to add a new item.
    /// </summary>
    public ICommand AddCommand { get; set; }

    /// <summary>
    /// Reset items command.
    /// </summary>
    public ICommand ResetCommand { get; set; }

    /// <summary>
    /// Command to edit an item.
    /// </summary>
    public ICommand EditCommand { get; set; }

    /// <summary>
    /// Command to delete an item.
    /// </summary>
    public ICommand DeleteCommand { get; set; }

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
        AddCommand = new AsyncCommand(async () => await AddItem());
        ResetCommand = new AsyncCommand(async () => await ResetItems());
        EditCommand = new AsyncCommand<GymObject>(async gymObject => await EditItem(gymObject));
        DeleteCommand = new AsyncCommand<GymObject>(async gymObject => await DeleteItem(gymObject));
    }

    // ---------------------------------------------------------------------------------------------
    internal async Task DisplayList()
    {
        Title = $"{GymObjectTypeName}s";

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

        // EditModeChanged();
    }

    /// <summary>
    /// Initialize the list of gym objects.
    /// </summary>
    private void DisplayList<T, TDrawable>(List<T> gymObjects)
        where T : GymObject
        where TDrawable : GymObjectDrawable, new()
    {
        var maxWeight = gymObjects.Last().Weight;

        Drawables = new List<GymObjectDrawable>();
        foreach (var gymObject in gymObjects)
        {
            // Construct the drawable.
            var drawable = new TDrawable
            {
                GymObject = gymObject,
                Width = 200,
                MinWidth = 50,
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
                // _isGymObjectTypeNameSet = true;
                break;

            case nameof(EditMode):
                // EditModeCheckBox.IsChecked = EditMode;
                // EditModeChanged();
                // _isEditModeSet = true;
                break;
        }
    }

    // ---------------------------------------------------------------------------------------------
    // Command methods.

    private async Task AddItem()
    {
        await Shell.Current.GoToAsync($"edit?type={GymObjectTypeName}");
    }

    private async Task ResetItems()
    {
        await Shell.Current.GoToAsync($"reset?type={GymObjectTypeName}");
    }

    private async Task EditItem(GymObject gymObject)
    {
        await Shell.Current.GoToAsync($"edit?type={GymObjectTypeName}&id={gymObject.Id}");
    }

    private async Task DeleteItem(GymObject gymObject)
    {
        await Shell.Current.GoToAsync($"delete?type={GymObjectTypeName}&id={gymObject.Id}");
    }

    // ---------------------------------------------------------------------------------------------
    // Old code.

    /*
    /// <summary>
    /// Initialize the list of gym objects.
    /// </summary>
    private void DisplayList<T, TDrawable>(List<T> gymObjects)
        where T : GymObject
        where TDrawable : GymObjectDrawable, new()
    {
        // Clear the grid.
        MauiUtilities.ClearGrid(ListGrid, true, true);

        // Set up the column definitions, which will vary with device orientation.
        ListGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nItemCols = PageLayout.GetNumColumns();
        var nGridCols = nItemCols * 2;
        for (var c = 0; c < nItemCols; c++)
        {
            // Add column for the graphic.
            ListGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            // Add column for the checkbox and icon buttons.
            ListGrid.ColumnDefinitions.Add(
                new ColumnDefinition(new GridLength(_IconButtonsLayoutWidth)));
        }

        // Add the instructions label at the top. The text and font size will be set later.
        ListGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        InstructionsLabel = new Label().ColumnSpan(nGridCols);
        ListGrid.Add(InstructionsLabel, 0, 0);

        // Get the maximum bar weight.
        var maxWeight = gymObjects.Last().Weight;

        // Calculate the maximum graphic width.
        var maxWidth = (int)(ListGrid.Width - ListGrid.Padding.Left - ListGrid.Padding.Right);
        if (nItemCols == 2)
        {
            maxWidth = (maxWidth - (int)ListGrid.ColumnSpacing) / 2;
        }
        maxWidth = maxWidth - (int)ListGrid.ColumnSpacing - _IconButtonsLayoutWidth - 30;

        // Get the icon styles.
        var editIconButtonStyle = MauiUtilities.LookupStyle("EditIconButtonStyle");
        var deleteIconButtonStyle = MauiUtilities.LookupStyle("DeleteIconButtonStyle");

        var rowNum = 1;
        var colNum = 0;
        foreach (var gymObject in gymObjects)
        {
            // Construct the drawable.
            var drawable = new TDrawable
            {
                GymObject = gymObject,
                MaxWidth = maxWidth,
                MaxWeight = maxWeight,
            };

            // If we're at the start of a new row, create one and add it to the grid.
            if (colNum == 0)
            {
                var rowHeight = Math.Max(drawable.Height, _IconButtonHeight);
                ListGrid.RowDefinitions.Add(new RowDefinition(new GridLength(rowHeight)));
            }

            // Add the graphic.
            var gymObjectGraphic = drawable.CreateGraphicsView();
            ListGrid.Add(gymObjectGraphic, colNum, rowNum);

            // Add the checkbox to the grid.
            var cb = new CheckBox
            {
                IsChecked = gymObject.Enabled,
                HorizontalOptions = LayoutOptions.Center,
            };
            cb.CheckChanged += EnabledCheckBox_OnCheckChanged;
            ListGrid.Add(cb, colNum + 1, rowNum);

            // Link the checkbox to the bar.
            _cbObjectMap[cb] = gymObject;

            // Create a horizontal stack for the edit and delete icon buttons.
            var stack = new HorizontalStackLayout { Spacing = 5 };

            // Add the edit button to the stack.
            var editBtn = new Button
            {
                Style = editIconButtonStyle,
            };
            editBtn.Clicked += EditIcon_OnClicked;
            stack.Add(editBtn);

            // Add the delete button to the stack.
            var deleteBtn = new Button
            {
                Style = deleteIconButtonStyle,
            };
            deleteBtn.Clicked += DeleteIcon_OnClicked;
            stack.Add(deleteBtn);

            // Link the icon button group to the bar.
            _stackObjectMap[stack] = gymObject;

            // Add the stack to the grid.
            ListGrid.Add(stack, colNum + 1, rowNum);

            // Next position.
            colNum += 2;
            if (colNum == nGridCols)
            {
                rowNum++;
                colNum = 0;
            }
        }
    }
    */
}
