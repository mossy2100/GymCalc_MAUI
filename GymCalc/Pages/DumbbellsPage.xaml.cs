using System.Globalization;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Graphics;
using GymCalc.Utilities;

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
    }

    protected override async void OnAppearing()
    {
        if (DumbbellsGrid.RowDefinitions.Count == 1)
        {
            await DisplayDumbbells();
        }
    }

    /// <summary>
    /// Initialize the list of dumbbells.
    /// </summary>
    private async Task DisplayDumbbells()
    {
        // Get the dumbbells.
        var dumbbells = await DumbbellRepository.GetAll();

        // Display them all in a table with checkboxes.
        var rowNum = 1;
        var colNum = 0;
        const int rowHeight = 50;
        foreach (var dumbbell in dumbbells)
        {
            if (colNum == 0)
            {
                // Add a new row to the grid.
                DumbbellsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(rowHeight)));
            }

            // Get the dumbbell background.
            var dumbbellGraphic = new GraphicsView
            {
                Drawable = new DumbbellGraphic(),
                HeightRequest = rowHeight,
                WidthRequest = 100,
            };
            DumbbellsGrid.Add(dumbbellGraphic, colNum, rowNum);

            // Add the dumbbell weight text.
            var labelText = dumbbell.Weight.ToString(CultureInfo.InvariantCulture);
            var label = new Label
            {
                FormattedText = TextUtility.StyleText(labelText, "DumbbellLabel"),
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            DumbbellsGrid.Add(label, colNum, rowNum);

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
            if (colNum == 2)
            {
                rowNum++;
                colNum = 0;
            }
            else
            {
                colNum = 2;
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
