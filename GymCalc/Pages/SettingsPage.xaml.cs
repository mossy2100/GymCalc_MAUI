using InputKit.Shared.Controls;

namespace GymCalc.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        Units.SelectedItem = Preferences.Default.Get("Units", "kg");
    }

    private void OnUnitsSelectedItemChanged(object sender, EventArgs e)
    {
        var rbg = (RadioButtonGroupView)sender;
        Preferences.Default.Set("Units", (string)rbg.SelectedItem);
    }
}
