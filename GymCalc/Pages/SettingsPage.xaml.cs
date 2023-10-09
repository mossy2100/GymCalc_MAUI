using Galaxon.Core.Enums;
using GymCalc.Utilities;
using InputKit.Shared.Controls;

namespace GymCalc.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        UnitsRadio.SelectedItem = UnitsUtility.GetDefault().GetDescription();
    }

    private void OnUnitsSelectedItemChanged(object sender, EventArgs e)
    {
        var rbg = (RadioButtonGroupView)sender;
        Preferences.Default.Set("Units", (string)rbg.SelectedItem);
    }
}
