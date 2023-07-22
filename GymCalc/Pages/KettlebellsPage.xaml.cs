using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Graphics;
using GymCalc.Graphics.Objects;
using GymCalc.Utilities;

namespace GymCalc.Pages;

public partial class KettlebellsPage : ContentPage
{
    /// <summary>
    /// Dictionary mapping checkboxes to kettlebells.
    /// </summary>
    private readonly Dictionary<CheckBox, Kettlebell> _cbKettlebellMap = new ();

    private bool _kettlebellsDisplayed;

    public KettlebellsPage()
    {
        InitializeComponent();
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        if (!_kettlebellsDisplayed)
        {
            await DisplayKettlebells();
            _kettlebellsDisplayed = true;
        }
    }

    private async void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        MauiUtilities.ClearGrid(KettlebellsGrid, true, true);
        await DisplayKettlebells();
    }

    /// <summary>
    /// Initialize the list of kettlebells.
    /// </summary>
    private async Task DisplayKettlebells()
    {
        // Get the kettlebells.
        var kettlebells = await KettlebellRepository.GetAll();

        // Set up the columns.
        KettlebellsGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nCols = App.GetNumColumns() * 4;
        for (var c = 0; c < nCols / 2; c++)
        {
            // Add 2 columns to the grid.
            KettlebellsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            KettlebellsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        // Set the stack height manually, because it doesn't resize automatically.
        var nRows = (int)double.Ceiling(kettlebells.Count / (nCols / 2.0));
        KettlebellsStackLayout.HeightRequest =
            (KettlebellGraphic.Height + App.Spacing) * nRows + App.DoubleSpacing;

        // Display the kettlebells in a table with checkboxes.
        var rowNum = 0;
        var colNum = 0;
        foreach (var kettlebell in kettlebells)
        {
            if (colNum == 0)
            {
                // Add a new row to the grid.
                KettlebellsGrid.RowDefinitions.Add(
                    new RowDefinition(new GridLength(KettlebellGraphic.Height)));
            }

            // Draw the kettlebell.
            var kettlebellGraphic = GraphicsFactory.CreateKettlebellGraphic(kettlebell);
            KettlebellsGrid.Add(kettlebellGraphic, colNum, rowNum);

            // Add the checkbox.
            var cb = new CheckBox
            {
                IsChecked = kettlebell.Enabled,
            };
            cb.CheckedChanged += OnKettlebellCheckboxChanged;
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
