using System.Runtime.CompilerServices;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Constants;
using GymCalc.Graphics;
using GymCalc.Graphics.Drawables;
using GymCalc.Utilities;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
public partial class ListPage : ContentPage
{
    private string _gymObjectTypeName;

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set
        {
            _gymObjectTypeName = value;
            OnPropertyChanged();
        }
    }

    private bool _editMode;

    /// <summary>
    /// Dictionary mapping checkboxes to gym objects.
    /// </summary>
    private readonly Dictionary<CheckBox, GymObject> _cbObjectMap = new ();

    /// <summary>
    /// Dictionary mapping horizontal stacks (with icon buttons) to gym objects.
    /// </summary>
    private readonly Dictionary<HorizontalStackLayout, GymObject> _stackObjectMap = new ();

    public ListPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    /// <inheritdoc />
    protected override async void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(GymObjectTypeName))
        {
            Title = $"{GymObjectTypeName}s";
            await DisplayList();
        }
    }

    private async Task DisplayList()
    {
        var units = Units.GetPreferred();
        switch (GymObjectTypeName)
        {
            case GymObjectType.Bar:
                var bars = await BarRepository.GetInstance().GetAll(units);
                DisplayList<Bar, BarDrawable>(bars, BarDrawable.Height);
                break;

            case GymObjectType.Plate:
                var plates = await PlateRepository.GetInstance().GetAll(units);
                DisplayList<Plate, PlateDrawable>(plates, PlateDrawable.Height);
                break;

            case GymObjectType.Dumbbell:
                var dumbbells = await DumbbellRepository.GetInstance().GetAll(units);
                DisplayList<Dumbbell, DumbbellDrawable>(dumbbells, DumbbellDrawable.Height);
                break;

            case GymObjectType.Kettlebell:
                var kettlebells = await KettlebellRepository.GetInstance().GetAll(units);
                DisplayList<Kettlebell, KettlebellDrawable>(kettlebells, KettlebellDrawable.Height);
                break;
        }
    }

    /// <summary>
    /// Initialize the list of gym objects.
    /// </summary>
    private void DisplayList<T, TDrawable>(List<T> gymObjects, int graphicHeight)
        where T : GymObject
        where TDrawable : GymObjectDrawable, new()
    {
        // Set the label text.
        ListLabel.Text =
            $"Select which {GymObjectTypeName.ToLower()} weights ({Units.GetPreferred()}) are available:";

        // Clear the grid.
        MauiUtilities.ClearGrid(ListGrid, true, true);

        // Set up the column definitions, which will vary with device orientation.
        ListGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nItemCols = PageLayout.GetNumColumns()
            * (GymObjectTypeName is GymObjectType.Dumbbell or GymObjectType.Kettlebell ? 2 : 1);
        var nGridCols = nItemCols * 2;
        for (var c = 0; c < nItemCols; c++)
        {
            // Add column for the graphic.
            ListGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            // Add column for the checkbox and icon buttons.
            ListGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(70)));
        }

        // Calculate and set the stack height because it doesn't resize automatically.
        var nRows = (int)double.Ceiling(gymObjects.Count / (double)nItemCols);
        var gridHeight =
            (graphicHeight + PageLayout.DoubleSpacing) * nRows + (PageLayout.DoubleSpacing * 2);
        ListStack.HeightRequest = ListLabel.Height + gridHeight + ListButtons.Height;

        // Get the maximum bar weight.
        var maxWeight = gymObjects.Last().Weight;

        // Get the icon styles.
        var editIconButtonStyle = MauiUtilities.LookupStyle("EditIconButtonStyle");
        var deleteIconButtonStyle = MauiUtilities.LookupStyle("DeleteIconButtonStyle");

        var rowNum = 0;
        var colNum = 0;
        foreach (var gymObject in gymObjects)
        {
            // If we're at the start of a new row, create one and add it to the grid.
            if (colNum == 0)
            {
                ListGrid.RowDefinitions.Add(new RowDefinition(new GridLength(graphicHeight)));
            }

            // Add the graphic.
            var drawable = new TDrawable
            {
                GymObject = gymObject,
                MaxWeight = maxWeight
            };
            var gymObjectGraphic = drawable.CreateGraphic();
            ListGrid.Add(gymObjectGraphic, colNum, rowNum);

            // Add the checkbox to the grid.
            var cb = new CheckBox
            {
                IsChecked = gymObject.Enabled,
                IsVisible = !_editMode,
            };
            cb.CheckChanged += OnCheckboxChanged;
            ListGrid.Add(cb, colNum + 1, rowNum);

            // Link the checkbox to the bar.
            _cbObjectMap[cb] = gymObject;

            // Create a horizontal stack for the edit and delete icon buttons.
            var stack = new HorizontalStackLayout
            {
                Spacing = 5,
                IsVisible = _editMode,
            };

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

    /// <summary>
    /// When the checkbox next to a bar is changed, update the database.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnCheckboxChanged(object sender, EventArgs e)
    {
        // Update the bar's Enabled state.
        var cb = (CheckBox)sender;
        var gymObject = _cbObjectMap[cb];
        gymObject.Enabled = cb.IsChecked;
        var db = Database.GetConnection();
        await db.UpdateAsync(gymObject);
    }

    #region Event handlers

    private async void AddButton_OnClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"edit?type={GymObjectTypeName}");
    }

    private void EditButton_OnClicked(object sender, EventArgs e)
    {
        foreach (var (cb, gymObject) in _cbObjectMap)
        {
            cb.IsVisible = false;
        }
        foreach (var (stack, gymObject) in _stackObjectMap)
        {
            stack.IsVisible = true;
        }
        EditButton.IsVisible = false;
        ViewButton.IsVisible = true;
        _editMode = true;
    }

    private void ViewButton_OnClicked(object sender, EventArgs e)
    {
        foreach (var (cb, gymObject) in _cbObjectMap)
        {
            cb.IsVisible = true;
        }
        foreach (var (stack, gymObject) in _stackObjectMap)
        {
            stack.IsVisible = false;
        }
        EditButton.IsVisible = true;
        ViewButton.IsVisible = false;
        _editMode = false;
    }

    private async void ResetButton_OnClicked(object sender, EventArgs e)
    {
        // Go to the reset confirmation page.
        await Shell.Current.GoToAsync($"reset?type={GymObjectTypeName}");
    }

    private async void EditIcon_OnClicked(object sender, EventArgs e)
    {
        // Get the gym object id and go to the edit form.
        var editBtn = (Button)sender;
        var stack = (HorizontalStackLayout)editBtn.Parent;
        var gymObject = _stackObjectMap[stack];
        await Shell.Current.GoToAsync($"edit?type={GymObjectTypeName}&id={gymObject.Id}");
    }

    private async void DeleteIcon_OnClicked(object sender, EventArgs e)
    {
        // Get the gym object id and go to the delete confirmation page.
        var deleteBtn = (Button)sender;
        var stack = (HorizontalStackLayout)deleteBtn.Parent;
        var gymObject = _stackObjectMap[stack];
        await Shell.Current.GoToAsync($"delete?type={GymObjectTypeName}&id={gymObject.Id}");
    }

    #endregion Event handlers
}
