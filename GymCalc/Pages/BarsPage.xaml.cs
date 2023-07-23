using Microsoft.Maui.Controls.Shapes;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Graphics;
using GymCalc.Graphics.Drawables;
using GymCalc.Utilities;

namespace GymCalc.Pages;

public partial class BarsPage : ContentPage
{
    /// <summary>
    /// Dictionary mapping checkboxes to bars.
    /// </summary>
    private readonly Dictionary<CheckBox, Bar> _cbBarMap = new ();

    private bool _barsDisplayed;

    public BarsPage()
    {
        InitializeComponent();
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        if (!_barsDisplayed)
        {
            await DisplayBars();
            _barsDisplayed = true;
        }
    }

    private async void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        MauiUtilities.ClearGrid(BarsGrid, true, true);
        await DisplayBars();
    }

    /// <summary>
    /// Initialize the list of bars.
    /// </summary>
    private async Task DisplayBars()
    {
        // Get the bars.
        var bars = await BarRepository.GetAll();

        // Get the style.
        var barLabelStyle = MauiUtilities.LookupStyle("BarLabelStyle");

        // Set up the columns.
        BarsGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nCols = PageLayout.GetNumColumns() * 2;
        for (var c = 0; c < nCols / 2; c++)
        {
            // Add 2 columns to the grid.
            BarsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            BarsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        // Set the stack height manually, because it doesn't resize automatically.
        var nRows = (int)double.Ceiling(bars.Count / (nCols / 2.0));
        BarsStackLayout.HeightRequest =
            (BarDrawable.Height + PageLayout.DoubleSpacing) * nRows + PageLayout.DoubleSpacing;

        // Get the maximum bar weight.
        var maxBarWeight = bars.Last().Weight;

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

            // Add the checkbox.
            var cb = new CheckBox
            {
                IsChecked = bar.Enabled,
            };
            cb.CheckedChanged += OnBarCheckboxChanged;
            BarsGrid.Add(cb, colNum + 1, rowNum);

            // Remember the bar weight in the lookup table.
            _cbBarMap[cb] = bar;

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
}
