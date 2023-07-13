using System.Globalization;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Utilities;
using Microsoft.Maui.Controls.Shapes;

namespace GymCalc.Pages;

public partial class PlatesPage : ContentPage
{
    /// <summary>
    /// Dictionary mapping checkboxes to plates.
    /// </summary>
    private Dictionary<CheckBox, Plate> _cbPlateMap = new ();

    public PlatesPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        if (PlatesGrid.RowDefinitions.Count == 1)
        {
            await DisplayPlates();
        }
    }

    /// <summary>
    /// Initialize the list of plates.
    /// </summary>
    private async Task DisplayPlates()
    {
        // Get all the plates, ordered by weight.
        var plates = await PlateRepository.GetAll();

        var rowNum = 1;
        var rowDefinition = new RowDefinition(new GridLength(30));
        foreach (var plate in plates)
        {
            PlatesGrid.RowDefinitions.Add(rowDefinition);
            AddPlateToGrid(plate, PlatesGrid, 0, rowNum);

            // Add the checkbox.
            var cb = new CheckBox
            {
                IsChecked = plate.Enabled,
            };
            cb.CheckedChanged += OnPlateCheckboxChanged;
            PlatesGrid.Add(cb, 1, rowNum);

            // Link the checkbox to the plate in the lookup table.
            _cbPlateMap[cb] = plate;

            // Next row.
            rowNum++;
        }
    }

    internal static void AddPlateToGrid(Plate plate, Grid platesGrid, int columnNum, int rowNum)
    {
        // Get the colors.
        var bgColor = Color.Parse(plate.Color);
        var textColor = bgColor.GetTextColor();

        // Add the plate background.
        var plateWidth = 50 + plate.Weight / 25 * 250;
        var rect = new Rectangle
        {
            RadiusX = 4,
            RadiusY = 4,
            HeightRequest = 30,
            WidthRequest = plateWidth,
            Fill = bgColor.AddLuminosity(-0.1f),
        };
        platesGrid.Add(rect, columnNum, rowNum);

        // Add the plate edge.
        var rect2 = new Rectangle
        {
            RadiusX = 0,
            RadiusY = 0,
            HeightRequest = 22,
            WidthRequest = plateWidth,
            Fill = bgColor,
        };
        platesGrid.Add(rect2, columnNum, rowNum);

        // Add the plate weight text.
        var label = new Label
        {
            Text = plate.Weight.ToString(CultureInfo.InvariantCulture),
            TextColor = textColor,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.Center
        };
        platesGrid.Add(label, columnNum, rowNum);
    }

    /// <summary>
    /// When the checkbox next to a plate is changed, update the database.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnPlateCheckboxChanged(object sender, EventArgs e)
    {
        // Update the plate's Enabled state.
        var cb = (CheckBox)sender;
        var plate = _cbPlateMap[cb];
        plate.Enabled = cb.IsChecked;
        var db = Database.GetConnection();
        await db.UpdateAsync(plate);
    }

    // private void AddNewPlateWeightClicked(object sender, EventArgs e)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // private void SavePlateWeightsClicked(object sender, EventArgs e)
    // {
    //     throw new NotImplementedException();
    // }
}
