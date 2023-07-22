using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Graphics;
using GymCalc.Graphics.Objects;
using GymCalc.Utilities;

namespace GymCalc.Pages;

public partial class DumbbellsPage : ContentPage
{
    /// <summary>
    /// Dictionary mapping checkboxes to dumbbells.
    /// </summary>
    private readonly Dictionary<CheckBox, Dumbbell> _cbDumbbellMap = new ();

    private bool _dumbbellsDisplayed;

    public DumbbellsPage()
    {
        InitializeComponent();
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        if (!_dumbbellsDisplayed)
        {
            await DisplayDumbbells();
            _dumbbellsDisplayed = true;
        }
    }

    private async void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        MauiUtilities.ClearGrid(DumbbellsGrid, true, true);
        await DisplayDumbbells();
    }

    /// <summary>
    /// Initialize the list of dumbbells.
    /// </summary>
    private async Task DisplayDumbbells()
    {
        // Get the dumbbells.
        var dumbbells = await DumbbellRepository.GetAll();

        // Set up the columns.
        DumbbellsGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nCols = App.GetNumColumns() * 4;
        for (var c = 0; c < nCols / 2; c++)
        {
            // Add 2 columns to the grid.
            DumbbellsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            DumbbellsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        // Set the stack height manually, because it doesn't resize automatically.
        var nRows = (int)double.Ceiling(dumbbells.Count / (nCols / 2.0));
        DumbbellsStackLayout.HeightRequest =
            (DumbbellGraphic.Height + App.DoubleSpacing) * nRows + App.DoubleSpacing;

        // Display the dumbbells in a table with checkboxes.
        var rowNum = 0;
        var colNum = 0;
        foreach (var dumbbell in dumbbells)
        {
            if (colNum == 0)
            {
                // Add a new row to the grid.
                DumbbellsGrid.RowDefinitions.Add(
                    new RowDefinition(new GridLength(DumbbellGraphic.Height)));
            }

            // Add the dumbbell graphic.
            var dumbbellGraphic = GraphicsFactory.CreateDumbbellGraphic(dumbbell);
            DumbbellsGrid.Add(dumbbellGraphic, colNum, rowNum);

            // Add the checkbox.
            var cb = new CheckBox
            {
                IsChecked = dumbbell.Enabled,
            };
            cb.CheckedChanged += OnDumbbellCheckboxChanged;
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
}
