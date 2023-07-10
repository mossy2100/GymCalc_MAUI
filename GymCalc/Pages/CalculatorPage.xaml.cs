using System.Globalization;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Utilities;

namespace GymCalc.Pages;

public partial class CalculatorPage : ContentPage
{
    private double MaxWeight;

    private double BarWeight;

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
        var initialSelectedValue =
            initialSelectedIndex != -1 ? BarWeightPicker.Items[initialSelectedIndex] : null;

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
            var weightString = Bar.DEFAULT_WEIGHT.ToString(CultureInfo.InvariantCulture);
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
        BarWeight = double.Parse(BarWeightPicker.Items[BarWeightPicker.SelectedIndex]);
        var maxWeightOk = double.TryParse(MaxWeightEntry.Text, out MaxWeight);
        if (!maxWeightOk || MaxWeight < BarWeight)
        {
            ResultsLabel.Text = "Make sure the max weight is greater than or equal to the bar weight.";
            ResultsLabel.TextColor = Colors.Red;
            return;
        }

        // Calculate and display the results.
        var results = await CalcPlates.CalculateResults(MaxWeight, BarWeight);
        DisplayResults(results);
    }

    private void DisplayResults(Dictionary<double, PlatesResult> results)
    {
        // Update the message.
        ResultsLabel.Text = "";
        // ResultsLabel.TextColor = Colors.Black;

        // Clear the old results.
        while (CalculatorLayout.Count > 3)
        {
            CalculatorLayout.RemoveAt(3);
        }

        var rowDef = new RowDefinition();

        foreach ((double percent, PlatesResult platesResult) in results)
        {
            var textGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection(
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto)
                ),
                ColumnDefinitions = new ColumnDefinitionCollection(
                    new ColumnDefinition(new GridLength(2, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star))
                ),
                RowSpacing = 5,
                ColumnSpacing = 5
            };
            var percentLabel = new Label()
            {
                Text = $"{percent}%",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold
            };
            textGrid.Add(percentLabel, 0, 0);

            var totalWeightLabel = new Label()
            {
                Text = "Total including bar:",
                FontSize = 16,
            };
            textGrid.Add(totalWeightLabel, 0, 1);

            var totalWeight = MaxWeight * percent / 100.0;
            var totalWeightAmount = new Label()
            {
                Text = $"{totalWeight:F2} kg",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold
            };
            textGrid.Add(totalWeightAmount, 1, 1);

            var targetWeightLabel = new Label()
            {
                Text = "Ideal plates per each end:",
                FontSize = 16,
            };
            textGrid.Add(targetWeightLabel, 0, 2);

            var targetWeightAmount = new Label()
            {
                Text = $"{platesResult.TargetWeight:F2} kg",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold
            };
            textGrid.Add(targetWeightAmount, 1, 2);

            var actualWeightLabel = new Label()
            {
                Text = "Plates to use:",
                FontSize = 16,
            };
            textGrid.Add(actualWeightLabel, 0, 3);

            var actualWeightAmount = new Label()
            {
                Text = $"{platesResult.ActualWeight:F2} kg",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold
            };
            textGrid.Add(actualWeightAmount, 1, 3);

            CalculatorLayout.Add(textGrid);

            // Construct the plates grid and add it to the layout.
            var platesGrid = new Grid { Padding = new Thickness(0, 0, 0, 20) };
            var sortedPlates = platesResult.Plates.OrderBy(p => p.Weight).ToList();
            var j = 0;
            for (var p = 0; p < sortedPlates.Count; p++)
            {
                platesGrid.RowDefinitions.Add(rowDef);
                var plate = sortedPlates[p];
                PlatesPage.AddPlateToGrid(plate, platesGrid, 0, j);
                j++;
            }
            CalculatorLayout.Add(platesGrid);
        }
    }
}
