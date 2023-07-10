using System.Globalization;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Utilities;
using Microsoft.Maui.Controls.Shapes;

namespace GymCalc.Pages;

public partial class BarsPage : ContentPage
{
    public BarsPage()
    {
        InitializeComponent();
        InitializeBars();
    }

    /// <summary>
    /// Dictionary mapping checkboxes to bars.
    /// </summary>
    private Dictionary<CheckBox, Bar> cbBarMap = new ();

    /// <summary>
    /// Initialize the list of bars.
    /// </summary>
    private async void InitializeBars()
    {
        // Ensure table exists.
        await BarRepository.InitializeTable();

        // Get the bars.
        var db = Database.GetConnection();
        var bars = await db.Table<Bar>().OrderBy(b => b.Weight).ToListAsync();

        // Get the steel bar gradient brush.
        var brush = ColorUtility.GetSteelBarBrush();

        var rowNum = 1;
        foreach (var bar in bars)
        {
            // Add a new row to the grid.
            BarsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(30)));

            // Add the bar background.
            var barLength = 50 + bar.Weight / 25 * 250;
            var rect = new Rectangle
            {
                RadiusX = 0,
                RadiusY = 0,
                HeightRequest = 22,
                WidthRequest = barLength,
                Fill = brush
            };
            BarsGrid.Add(rect, 0, rowNum);

            // Add the bar weight text.
            var label = new Label
            {
                Text = bar.Weight.ToString(CultureInfo.InvariantCulture),
                TextColor = Colors.Black,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            BarsGrid.Add(label, 0, rowNum);

            // Add the checkbox.
            var cb = new CheckBox
            {
                IsChecked = bar.Enabled,
                // Color = Colors.White
            };
            cb.CheckedChanged += OnBarCheckboxChanged;
            BarsGrid.Add(cb, 1, rowNum);

            // Remember the bar weight in the lookup table.
            cbBarMap[cb] = bar;

            // Next row.
            rowNum++;
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
        var bar = cbBarMap[cb];
        bar.Enabled = cb.IsChecked;
        var db = Database.GetConnection();
        await db.UpdateAsync(bar);
    }
}
