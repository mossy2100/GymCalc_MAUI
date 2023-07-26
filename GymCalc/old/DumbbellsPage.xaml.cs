using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Constants;
using GymCalc.Graphics;
using GymCalc.Graphics.Drawables;
using GymCalc.Utilities;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace GymCalc.Pages;

public partial class DumbbellsPage : ContentPage
{
    /// <summary>
    /// Dictionary mapping checkboxes to dumbbells.
    /// </summary>
    private readonly Dictionary<CheckBox, Dumbbell> _cbDumbbellMap = new ();

    public DumbbellsPage()
    {
        InitializeComponent();
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        DumbbellsLabel.Text =
            $"Select which dumbbell weights ({Units.GetPreferred()}) are available:";
        await DisplayDumbbells();
    }

    private async void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        await DisplayDumbbells();
    }

    /// <summary>
    /// Initialize the list of dumbbells.
    /// </summary>
    private async Task DisplayDumbbells()
    {
        // Clear the grid.
        MauiUtilities.ClearGrid(DumbbellsGrid, true, true);

        // Get the dumbbells.
        var dumbbells = await DumbbellRepository.GetAll(Units.GetPreferred());

        // Set up the columns.
        DumbbellsGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nCols = PageLayout.GetNumColumns() * 4;
        for (var c = 0; c < nCols / 2; c++)
        {
            // Add 2 columns to the grid.
            DumbbellsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            DumbbellsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        // Calculate and set the stack height because it doesn't resize automatically.
        var nRows = (int)double.Ceiling(dumbbells.Count / (nCols / 2.0));
        var gridHeight = (DumbbellDrawable.Height + PageLayout.DoubleSpacing) * nRows
            + PageLayout.DoubleSpacing;
        DumbbellsStack.HeightRequest = DumbbellsLabel.Height + gridHeight + DumbbellsButtons.Height;

        // Display the dumbbells in a table with checkboxes.
        var rowNum = 0;
        var colNum = 0;
        foreach (var dumbbell in dumbbells)
        {
            // If we're at the start of a new row, create one and add it to the grid.
            if (colNum == 0)
            {
                DumbbellsGrid.RowDefinitions.Add(
                    new RowDefinition(new GridLength(DumbbellDrawable.Height)));
            }

            // Add the dumbbell graphic.
            var dumbbellGraphic = DumbbellDrawable.CreateGraphic(dumbbell);
            DumbbellsGrid.Add(dumbbellGraphic, colNum, rowNum);

            // Add the checkbox.
            var cb = new CheckBox
            {
                IsChecked = dumbbell.Enabled,
            };
            cb.CheckChanged += OnDumbbellCheckboxChanged;
            DumbbellsGrid.Add(cb, colNum + 1, rowNum);

            // Remember the dumbbell weight in the lookup table.
            _cbDumbbellMap[cb] = dumbbell;

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
    /// When the checkbox next to a dumbbell is changed, update the database.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnDumbbellCheckboxChanged(object sender, EventArgs e)
    {
        // Update the dumbbell's Enabled state.
        var cb = (CheckBox)sender;
        var dumbbell = _cbDumbbellMap[cb];
        dumbbell.Enabled = cb.IsChecked;
        var db = Database.GetConnection();
        await db.UpdateAsync(dumbbell);
    }

    private async void AddButton_OnClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//edit?type=Dumbbell");
    }

    private void EditButton_OnClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ResetButton_OnClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}
