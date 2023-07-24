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

    private bool _platesDisplayed;

    public PlatesPage()
    {
        InitializeComponent();
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        if (!_platesDisplayed)
        {
            await DisplayPlates();
            _platesDisplayed = true;
        }
    }

    private async void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        MauiUtilities.ClearGrid(PlatesGrid, true, true);
        await DisplayPlates();
    }

    /// <summary>
    /// Initialize the list of plates.
    /// </summary>
    private async Task DisplayPlates()
    {
        // Get all the plates, ordered by weight.
        var plates = await PlateRepository.GetAll();

        // Set up the columns.
        PlatesGrid.ColumnDefinitions = new ColumnDefinitionCollection();
        var nCols = PageLayout.GetNumColumns() * 2;
        for (var c = 0; c < nCols / 2; c++)
        {
            // Add 2 columns to the grid.
            PlatesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            PlatesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        // Set the stack height manually, because it doesn't resize automatically.
        var nRows = (int)double.Ceiling(plates.Count / (nCols / 2.0));
        PlatesStackLayout.HeightRequest =
            (PlateDrawable.Height + PageLayout.DoubleSpacing) * nRows + PageLayout.DoubleSpacing;

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

    internal static void AddPlateToGrid(Plate plate, Grid platesGrid, int columnNum, int rowNum, double maxPlateWeight)
    {
        // Get the colors.
        var bgColor = Color.Parse(plate.Color);
        var textColor = bgColor.GetTextColor();

        // Get the style.
        var plateLabelStyle = MauiUtilities.LookupStyle("PlateLabelStyle");

        // Calculate the plate width.
        var maxPlateWidth = MauiUtilities.GetDeviceWidth() / PageLayout.GetNumColumns() * 0.75;
        var plateWidth = PlateDrawable.MinWidth +
            plate.Weight / maxPlateWeight * (maxPlateWidth - PlateDrawable.MinWidth);

        // Add the plate background.
        var rect = new Rectangle
        {
            RadiusX = PlateDrawable.CornerRadius,
            RadiusY = PlateDrawable.CornerRadius,
            HeightRequest = PlateDrawable.Height,
            WidthRequest = plateWidth,
            Fill = bgColor.AddLuminosity(-0.1f),
        };
        platesGrid.Add(rect, columnNum, rowNum);

        // Add the plate edge.
        var rect2 = new Rectangle
        {
            RadiusX = 0,
            RadiusY = 0,
            HeightRequest = PlateDrawable.InnerHeight,
            WidthRequest = plateWidth,
            Fill = bgColor,
        };
        platesGrid.Add(rect2, columnNum, rowNum);

        // Add the plate weight text.
        var label = new Label
        {
            FormattedText = TextUtility.CreateFormattedString($"{plate.Weight}", true, false,
                textColor, plateLabelStyle),
            // TextColor = textColor,
            // FontSize = 16,
            // FontAttributes = FontAttributes.Bold,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.Center,
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
