using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Graphics;
using GymCalc.Graphics.Drawables;
using GymCalc.Utilities;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace GymCalc.Pages;

public partial class BarsPage : ContentPage
{
    /// <summary>
    /// Dictionary mapping checkboxes to bars.
    /// </summary>
    private readonly Dictionary<CheckBox, Bar> _cbBarMap = new ();

    private readonly Dictionary<HorizontalStackLayout, Bar> _stackBarMap = new ();

    public BarsPage()
    {
        InitializeComponent();
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        BarsLabel.Text = $"Select which bar weights ({Units.GetPreferred()}) are available:";
        await DisplayBars();
    }

    private async void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        await DisplayBars();
    }

    /// <summary>
    /// Initialize the list of bars.
    /// </summary>
    private async Task DisplayBars()
    {
        // Clear the grid.
        MauiUtilities.ClearGrid(BarsGrid, true, true);

        // Get the bars.
        var bars = await BarRepository.GetAll(Units.GetPreferred());

        // Set up the columns.
        BarsGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nCols = PageLayout.GetNumColumns() * 2;
        for (var c = 0; c < nCols / 2; c++)
        {
            // Add 2 columns to the grid.
            BarsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            BarsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(70)));
        }

        // Calculate and set the stack height because it doesn't resize automatically.
        var nRows = (int)double.Ceiling(bars.Count / (nCols / 2.0));
        var gridHeight = (BarDrawable.Height + PageLayout.DoubleSpacing) * nRows
            + PageLayout.DoubleSpacing;
        BarsStack.HeightRequest = BarsLabel.Height + gridHeight + BarsButtons.Height;

        // Get the maximum bar weight.
        var maxBarWeight = bars.Last().Weight;

        var editIconButtonStyle = MauiUtilities.LookupStyle("EditIconButtonStyle");
        var deleteIconButtonStyle = MauiUtilities.LookupStyle("DeleteIconButtonStyle");

        var rowNum = 0;
        var colNum = 0;
        foreach (var bar in bars)
        {
            // If we're at the start of a new row, create one and add it to the grid.
            if (colNum == 0)
            {
                BarsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(BarDrawable.Height)));
            }

            // Add the bar graphic.
            var barGraphic = BarDrawable.CreateGraphic(bar, maxBarWeight);
            BarsGrid.Add(barGraphic, colNum, rowNum);

            // Add the checkbox to the grid.
            var cb = new CheckBox
            {
                IsChecked = bar.Enabled,
            };
            cb.CheckChanged += OnBarCheckboxChanged;
            BarsGrid.Add(cb, colNum + 1, rowNum);

            // Link the checkbox to the bar.
            _cbBarMap[cb] = bar;

            // Create a horizontal stack for the edit and delete icon buttons.
            var stack = new HorizontalStackLayout
            {
                Spacing = 5,
                IsVisible = false,
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
            _stackBarMap[stack] = bar;

            // Add the stack to the grid.
            BarsGrid.Add(stack, colNum + 1, rowNum);

            // Next position.
            colNum += 2;
            if (colNum == nCols)
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
    private async void OnBarCheckboxChanged(object sender, EventArgs e)
    {
        // Update the bar's Enabled state.
        var cb = (CheckBox)sender;
        var bar = _cbBarMap[cb];
        bar.Enabled = cb.IsChecked;
        var db = Database.GetConnection();
        await db.UpdateAsync(bar);
    }

    #region Event handlers

    private async void AddButton_OnClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//edit?type=bar");
    }

    private void EditButton_OnClicked(object sender, EventArgs e)
    {
        foreach (var (cb, bar) in _cbBarMap)
        {
            cb.IsVisible = false;
        }
        foreach (var (stack, bar) in _stackBarMap)
        {
            stack.IsVisible = true;
        }
        EditButton.IsVisible = false;
        ViewButton.IsVisible = true;
    }

    private void ViewButton_OnClicked(object sender, EventArgs e)
    {
        foreach (var (cb, bar) in _cbBarMap)
        {
            cb.IsVisible = true;
        }
        foreach (var (stack, bar) in _stackBarMap)
        {
            stack.IsVisible = false;
        }
        EditButton.IsVisible = true;
        ViewButton.IsVisible = false;
    }

    private async void ResetButton_OnClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//reset?class=Bar");
        throw new NotImplementedException();
    }

    private async void EditIcon_OnClicked(object sender, EventArgs e)
    {
        // Get the Bar Id and go to the edit form.
        var editBtn = (Button)sender;
        var stack = (HorizontalStackLayout)editBtn.Parent;
        var bar = _stackBarMap[stack];
        await Shell.Current.GoToAsync($"//edit?class=Bar&Id={bar.Id}");
    }

    private async void DeleteIcon_OnClicked(object sender, EventArgs e)
    {
        // Get the bar id.
        var id = 0;
        await Shell.Current.GoToAsync($"//delete?class=Bar&Id={id}");
    }

    #endregion Event handlers
}
