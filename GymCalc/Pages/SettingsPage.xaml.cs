using InputKit.Shared.Controls;

namespace GymCalc.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        UnitsRadio.SelectedItem = Units.GetPreferred();
    }

    private void OnUnitsSelectedItemChanged(object sender, EventArgs e)
    {
        var rbg = (RadioButtonGroupView)sender;
        Preferences.Default.Set("Units", (string)rbg.SelectedItem);
    }
}
