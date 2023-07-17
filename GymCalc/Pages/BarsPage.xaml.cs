using Microsoft.Maui.Controls.Shapes;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
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

    /// <summary>
    /// Initialize the list of bars.
    /// </summary>
    private async Task DisplayBars()
    {
        // Get the bars.
        var bars = await BarRepository.GetAll();

        // Get the steel bar gradient brush.
        var brush = GetBarBrush();

        // Get the style.
        var barLabelStyle = MauiUtilities.LookupStyle("BarLabelStyle");

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
                Fill = brush,
            };
            BarsGrid.Add(rect, 0, rowNum);

            // Add the bar weight text.
            var label = new Label
            {
                FormattedText = TextUtility.StyleText($"{bar.Weight}", barLabelStyle),
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            BarsGrid.Add(label, 0, rowNum);

            // Add the checkbox.
            var cb = new CheckBox
            {
                IsChecked = bar.Enabled,
            };
            cb.CheckedChanged += OnBarCheckboxChanged;
            BarsGrid.Add(cb, 1, rowNum);

            // Remember the bar weight in the lookup table.
            _cbBarMap[cb] = bar;

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
        var bar = _cbBarMap[cb];
        bar.Enabled = cb.IsChecked;
        var db = Database.GetConnection();
        await db.UpdateAsync(bar);
    }

    /// <summary>
    /// Create the steel bar gradient brush.
    /// </summary>
    /// <returns></returns>
    internal static Brush GetBarBrush()
    {
        var brush = new LinearGradientBrush { EndPoint = new Point(0, 1) };
        brush.GradientStops.Add(new GradientStop(Color.Parse("#aaa"), 0));
        brush.GradientStops.Add(new GradientStop(Colors.White, 0.5f));
        brush.GradientStops.Add(new GradientStop(Color.Parse("#aaa"), 1));
        return brush;
    }
}
