using Microsoft.Maui.Controls.Shapes;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Graphics;
using GymCalc.Graphics.Drawables;
using GymCalc.Utilities;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace GymCalc.Pages;

public partial class PlatesPage : ContentPage
{
    /// <summary>
    /// Dictionary mapping checkboxes to plates.
    /// </summary>
    private readonly Dictionary<CheckBox, Plate> _cbPlateMap = new ();

    public PlatesPage()
    {
        InitializeComponent();
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        PlatesLabel.Text =
            $"Select which plate weights ({Units.GetPreferred()}) are available:";
        await DisplayPlates();
    }

    private async void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        await DisplayPlates();
    }

    /// <summary>
    /// Initialize the list of plates.
    /// </summary>
    private async Task DisplayPlates()
    {
        // Clear the grid.
        MauiUtilities.ClearGrid(PlatesGrid, true, true);

        // Get all the plates, ordered by weight.
        var plates = await PlateRepository.GetAll(Units.GetPreferred());

        // Set up the columns.
        PlatesGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nCols = PageLayout.GetNumColumns() * 2;
        for (var c = 0; c < nCols / 2; c++)
        {
            // Add 2 columns to the grid.
            PlatesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            PlatesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        // Calculate and set the stack height because it doesn't resize automatically.
        var nRows = (int)double.Ceiling(plates.Count / (nCols / 2.0));
        var gridHeight = (PlateDrawable.Height + PageLayout.DoubleSpacing) * nRows
            + PageLayout.DoubleSpacing;
        PlatesStackLayout.HeightRequest = PlatesLabel.Height + gridHeight + PlatesButtons.Height;

        // Get the maximum plate weight.
        var maxPlateWeight = plates.Last().Weight;

        var rowNum = 0;
        var colNum = 0;
        foreach (var plate in plates)
        {
            // If we're at the start of a new row, create one and add it to the grid.
            if (colNum == 0)
            {
                PlatesGrid.RowDefinitions.Add(
                    new RowDefinition(new GridLength(PlateDrawable.Height)));
            }

            // Add plate graphic to grid.
            // AddPlateToGrid(plate, PlatesGrid, colNum, rowNum, maxPlateWeight);
            var plateGraphic = PlateDrawable.CreateGraphic(plate, maxPlateWeight);
            PlatesGrid.Add(plateGraphic, colNum, rowNum);

            // Add the checkbox.
            var cb = new CheckBox
            {
                IsChecked = plate.Enabled,
            };
            cb.CheckChanged += OnPlateCheckboxChanged;
            PlatesGrid.Add(cb, colNum + 1, rowNum);

            // Link the checkbox to the plate in the lookup table.
            _cbPlateMap[cb] = plate;

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

    private async void AddButton_OnClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//edit?type=plate");
    }

    private void EditButton_OnClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ResetButton_OnClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}
