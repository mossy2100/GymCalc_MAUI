using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Graphics;
using GymCalc.Graphics.Drawables;
using GymCalc.Utilities;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace GymCalc.Pages;

public partial class KettlebellsPage : ContentPage
{
    /// <summary>
    /// Dictionary mapping checkboxes to kettlebells.
    /// </summary>
    private readonly Dictionary<CheckBox, Kettlebell> _cbKettlebellMap = new ();

    public KettlebellsPage()
    {
        InitializeComponent();
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        KettlebellsGridLabel.Text =
            $"Select which kettlebell weights ({Units.GetUnits()}) are available:";
        await DisplayKettlebells();
    }

    private async void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        await DisplayKettlebells();
    }

    /// <summary>
    /// Initialize the list of kettlebells.
    /// </summary>
    private async Task DisplayKettlebells()
    {
        // Clear the grid.
        MauiUtilities.ClearGrid(KettlebellsGrid, true, true);

        // Get the kettlebells.
        var kettlebells = await KettlebellRepository.GetAll(Units.GetUnits());

        // Set up the columns.
        KettlebellsGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nCols = PageLayout.GetNumColumns() * 4;
        for (var c = 0; c < nCols / 2; c++)
        {
            // Add 2 columns to the grid.
            KettlebellsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            KettlebellsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        // Set the stack height manually, because it doesn't resize automatically.
        var nRows = (int)double.Ceiling(kettlebells.Count / (nCols / 2.0));
        KettlebellsStackLayout.HeightRequest =
            (KettlebellDrawable.Height + PageLayout.DoubleSpacing) * nRows
            + PageLayout.DoubleSpacing;

        // Display the kettlebells in a table with checkboxes.
        var rowNum = 0;
        var colNum = 0;
        foreach (var kettlebell in kettlebells)
        {
            // If we're at the start of a new row, create one and add it to the grid.
            if (colNum == 0)
            {
                // Add a new row to the grid.
                KettlebellsGrid.RowDefinitions.Add(
                    new RowDefinition(new GridLength(KettlebellDrawable.Height)));
            }

            // Draw the kettlebell.
            var kettlebellGraphic = KettlebellDrawable.CreateGraphic(kettlebell);
            KettlebellsGrid.Add(kettlebellGraphic, colNum, rowNum);

            // Add the checkbox.
            var cb = new CheckBox
            {
                IsChecked = kettlebell.Enabled,
            };
            cb.CheckChanged += OnKettlebellCheckboxChanged;
            KettlebellsGrid.Add(cb, colNum + 1, rowNum);

            // Remember the kettlebell weight in the lookup table.
            _cbKettlebellMap[cb] = kettlebell;

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
    /// When the checkbox next to a kettlebell is changed, update the database.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnKettlebellCheckboxChanged(object sender, EventArgs e)
    {
        // Update the enabled flag.
        var cb = (CheckBox)sender;
        var kettlebell = _cbKettlebellMap[cb];
        kettlebell.Enabled = cb.IsChecked;
        var db = Database.GetConnection();
        await db.UpdateAsync(kettlebell);
    }
}
