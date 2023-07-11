using System.Globalization;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using GymCalc.Utilities;

namespace GymCalc.Pages;

public partial class CalculatorPage : ContentPage
{
    private double _maxWeight;

    private double _barWeight;

    public CalculatorPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        // Update the bar weight picker whenever this page appears, because the bar weights may have
        // changed on the Bars page.
        ResetBarWeightPicker();
    }

    /// <summary>
    /// Reset the bar weight picker items.
    /// </summary>
    private void ResetBarWeightPicker()
    {
        // Get the current selected value.
        var initialSelectedIndex = BarWeightPicker.SelectedIndex;
        var initialSelectedValue = initialSelectedIndex != -1
            ? BarWeightPicker.Items[initialSelectedIndex]
            : null;

        // Reset the picker items.
        BarWeightPicker.Items.Clear();
        BarWeightPicker.SelectedIndex = -1;

        // Get the available bar weights.
        var db = Database.GetConnection();
        var bars = db.Table<Bar>()
            .Where(b => b.Enabled)
            .OrderBy(b => b.Weight)
            .ToListAsync()
            .Result;

        // Initialise the items in the bar weight picker.
        var i = 0;
        var valueSelected = false;
        foreach (var bar in bars)
        {
            // Add the picker item.
            var weightString = bar.Weight.ToString(CultureInfo.InvariantCulture);
            BarWeightPicker.Items.Add(weightString);

            // Try to select the same weight that was selected before.
            if (!valueSelected && weightString == initialSelectedValue)
            {
                BarWeightPicker.SelectedIndex = i;
                valueSelected = true;
            }

            i++;
        }

        // If the original selected bar weight is no longer present, try to select the default.
        if (!valueSelected)
        {
            var weightString = BarRepository.DEFAULT_WEIGHT.ToString(CultureInfo.InvariantCulture);
            for (i = 0; i < BarWeightPicker.Items.Count; i++)
            {
                // Default selection.
                if (BarWeightPicker.Items[i] == weightString)
                {
                    BarWeightPicker.SelectedIndex = i;
                    valueSelected = true;
                    break;
                }
            }
        }

        // If no bar weight has been selected yet, select the first one.
        if (!valueSelected)
        {
            BarWeightPicker.SelectedIndex = 0;
        }
    }

    private async void CalculateButtonClicked(object sender, EventArgs e)
    {
        // Get the weights from the calculator fields and validate them.
        _barWeight = double.Parse(BarWeightPicker.Items[BarWeightPicker.SelectedIndex]);
        var maxWeightOk = double.TryParse(MaxWeightEntry.Text, out _maxWeight);
        if (!maxWeightOk || _maxWeight < _barWeight)
        {
            ResultsLabel.Text = "Make sure the max weight is greater than or equal to the bar weight.";
            ResultsLabel.TextColor = Colors.Red;
            return;
        }

        // Calculate and display the results.
        var results = await CalcPlates.CalculateResults(_maxWeight, _barWeight);
        DisplayResults(results);
    }

    private void DisplayResults(Dictionary<double, List<double>> results)
    {
        // Clear the error message.
        ResultsLabel.Text = "";

        // Clear the old results.
        while (CalculatorLayout.Count > 3)
        {
            CalculatorLayout.RemoveAt(3);
        }

        // Get the available plates.
        var plates = PlateRepository.GetAllAvailable();

        foreach (var (percent, platesResult) in results)
        {
            var textGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection(
                    new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(2, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(2, GridUnitType.Star))
                ),
                RowDefinitions = new RowDefinitionCollection(
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto)
                ),
                ColumnSpacing = 15,
                RowSpacing = 15,
            };

            ///////////
            // Row 0.

            // Percentage heading.
            var percentLabel = new Label
            {
                FormattedText = TextUtility.BoldText($"{percent}%"),
                FontSize = 24,
            };
            textGrid.Add(percentLabel, 0);

            // Ideal heading.
            var idealHeading = new Label
            {
                FormattedText = TextUtility.BoldText("Ideal"),
                FontSize = 16,
                VerticalTextAlignment = TextAlignment.End,
            };
            textGrid.Add(idealHeading, 1);

            // Closest heading.
            var closestHeading = new Label
            {
                FormattedText = TextUtility.BoldText("Closest"),
                FontSize = 16,
                VerticalTextAlignment = TextAlignment.End,
            };
            textGrid.Add(closestHeading, 2);

            ///////////
            // Row 1.

            // Total including bar heading.
            var totalHeading = new Label
            {
                FormattedText = TextUtility.BoldText("Total including bar"),
                FontSize = 16,
            };
            textGrid.Add(totalHeading, 0, 1);

            // Ideal total weight.
            var idealTotal = _maxWeight * percent / 100.0;
            var idealPlates = (idealTotal - _barWeight) / 2;
            var idealTotalValue = new Label
            {
                FormattedText = TextUtility.NormalText($"{idealTotal:F2} kg"),
                FontSize = 16,
            };
            textGrid.Add(idealTotalValue, 1, 1);

            // Closest total weight.
            var closestPlates = platesResult.Sum();
            var closestTotal = 2 * closestPlates + _barWeight;
            var closestTotalValue = new Label
            {
                FormattedText = TextUtility.NormalText($"{closestTotal:F2} kg"),
                FontSize = 16,
            };
            textGrid.Add(closestTotalValue, 2, 1);

            ///////////
            // Row 2.

            // Plates per end heading.
            var platesPerEndHeading = new Label
            {
                FormattedText = TextUtility.BoldText("Plates per end"),
                FontSize = 16,
            };
            textGrid.Add(platesPerEndHeading, 0, 2);

            // Ideal plates weight.
            var idealPlatesValue = new Label
            {
                FormattedText = TextUtility.NormalText($"{idealPlates:F2} kg"),
                FontSize = 16,
            };
            textGrid.Add(idealPlatesValue, 1, 2);

            // Closest plates weight.
            var closestPlatesValue = new Label
            {
                FormattedText = TextUtility.NormalText($"{closestPlates:F2} kg"),
                FontSize = 16,
            };
            textGrid.Add(closestPlatesValue, 2, 2);

            CalculatorLayout.Add(textGrid);

            // Construct the plates grid and add it to the layout.
            var platesGrid = new Grid { Padding = new Thickness(0, 0, 0, 20) };
            var j = 0;
            platesResult.Sort();
            foreach (var plateWeight in platesResult)
            {
                platesGrid.RowDefinitions.Add(new RowDefinition());
                var plate = plates.First(p => p.Weight == plateWeight);
                PlatesPage.AddPlateToGrid(plate, platesGrid, 0, j);
                j++;
            }
            CalculatorLayout.Add(platesGrid);
        }
    }
}
