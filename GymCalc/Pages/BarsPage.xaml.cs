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

    private const int _barHeight = 22;

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

        // Get the steel bar gradient brush.
        var brush = GetBarBrush();

        // Get the style.
        var barLabelStyle = MauiUtilities.LookupStyle("BarLabelStyle");

        // Set up the columns.
        BarsGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nCols = App.GetNumColumns() * 2;
        for (var c = 0; c < nCols / 2; c++)
        {
            // Add 2 columns to the grid.
            BarsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            BarsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        // Set the stack height manually, because it doesn't resize automatically.
        var nRows = (int)double.Ceiling(bars.Count / (nCols / 2.0));
        BarsStackLayout.HeightRequest =
            (_barHeight + App.DoubleSpacing) * nRows + App.DoubleSpacing;

        // Get the min and max bar width.
        const int minBarWidth = 50;
        var maxBarWidth = MauiUtilities.GetDeviceWidth() / App.GetNumColumns() * 0.75;

        // Get the maximum bar weight.
        var maxBarWeight = bars.Last().Weight;

        var rowNum = 0;
        var colNum = 0;
        foreach (var bar in bars)
        {
            // Add a new row to the grid.
            BarsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(_barHeight)));

            // Calculate the bar width.
            var barWidth = minBarWidth + bar.Weight / maxBarWeight * (maxBarWidth - minBarWidth);

            // Add the bar background.
            var rect = new Rectangle
            {
                RadiusX = 0,
                RadiusY = 0,
                HeightRequest = _barHeight,
                WidthRequest = barWidth,
                Fill = brush,
            };
            BarsGrid.Add(rect, colNum, rowNum);

            // Add the bar weight text.
            var label = new Label
            {
                FormattedText = TextUtility.StyleText($"{bar.Weight}", barLabelStyle),
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            BarsGrid.Add(label, colNum, rowNum);

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
