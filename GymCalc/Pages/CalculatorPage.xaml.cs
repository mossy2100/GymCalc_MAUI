using System.Globalization;
using GymCalc.Data;
using GymCalc.Data.Models;

namespace GymCalc.Pages;

public partial class CalculatorPage : ContentPage
{
    public CalculatorPage()
    {
        InitializeComponent();

        // Initialise the items in the bar weight picker.
        var db = Database.GetConnection();
        var bars = db.Table<Bar>().ToListAsync().Result;
        var i = 0;
        var defaultBarSelected = false;
        foreach (var bar in bars)
        {
            // Add the picker item.
            BarWeight.Items.Add(bar.Weight.ToString(CultureInfo.InvariantCulture));

            // Default selection.
            if (bar.Weight == Bar.DEFAULT_WEIGHT)
            {
                BarWeight.SelectedIndex = i;
                defaultBarSelected = true;
            }

            i++;
        }

        // If the default bar weight could not be selected, select the first one in the picker.
        if (!defaultBarSelected)
        {
            BarWeight.SelectedIndex = 0;
        }
    }

    private void CalculateButton_Clicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}
