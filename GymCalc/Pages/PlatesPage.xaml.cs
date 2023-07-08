using System.Globalization;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using Microsoft.Maui.Controls.Shapes;

namespace GymCalc.Pages;

public partial class PlatesPage : ContentPage
{
    public PlatesPage()
    {
        InitializeComponent();
        InitializePlates();
    }

    /// <summary>
    /// Dictionary mapping checkboxes to plates.
    /// </summary>
    private Dictionary<CheckBox, Plate> _cbPlateMap = new ();

    /// <summary>
    /// Initialize the list of plates.
    /// </summary>
    private async void InitializePlates()
    {
        // Ensure table exists.
        await PlateRepository.InitializeTable();

        // Get the plates.
        var db = Database.GetConnection();
        var plates = await db.Table<Plate>().ToListAsync();

        var row = 1;
        foreach (var plate in plates)
        {
            PlatesGrid.RowDefinitions.Add(new RowDefinition(new GridLength(30)));

            // Get the colors.
            var bgColor = Color.FromInt(plate.Color);
            var textColor = bgColor.GetTextColor();

            // Add the plate background.
            var rect = new Rectangle
            {
                RadiusX = 5,
                RadiusY = 5,
                HeightRequest = 30,
                Fill = bgColor
            };
            PlatesGrid.Add(rect, 0, row);

            // Add the plate weight text.
            var label = new Label()
            {
                Text = plate.Weight.ToString(CultureInfo.InvariantCulture),
                TextColor = textColor,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            PlatesGrid.Add(label, 0, row);

            // Add the checkbox.
            var cb = new CheckBox
            {
                IsChecked = plate.Enabled,
                Color = Colors.White
            };
            cb.CheckedChanged += OnPlateCheckboxChanged;
            PlatesGrid.Add(cb, 1, row);

            // Remember the plate weight in the lookup table.
            _cbPlateMap[cb] = plate;

            // Next row.
            row++;
        }
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
