using GymCalc.Constants;
using GymCalc.Themes;
using InputKit.Shared.Controls;

namespace GymCalc.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        UnitsRadio.SelectedItem = Units.GetPreferred();
        ColorSchemeRadio.SelectedItem = Preferences.Default.Get("ColorScheme", Theme.Orange);
    }

    private void OnUnitsSelectedItemChanged(object sender, EventArgs e)
    {
        var rbg = (RadioButtonGroupView)sender;
        Preferences.Default.Set("Units", (string)rbg.SelectedItem);
    }

    private void OnColorSchemeSelectedItemChanged(object sender, EventArgs e)
    {
        // Get the theme they selected.
        var rbg = (RadioButtonGroupView)sender;
        var theme = (string)rbg.SelectedItem;

        // Record their preference.
        Preferences.Default.Set("ColorScheme", theme);

        // Update the theme.
        ICollection<ResourceDictionary> mergedDictionaries = Application.Current!.Resources.MergedDictionaries;
        if (mergedDictionaries != null)
        {
            foreach (var resourceDictionary in mergedDictionaries)
            {
                if (resourceDictionary is PinkTheme or OrangeTheme || resourceDictionary.Source.ToString().Contains("Theme.xaml"))
                {
                    mergedDictionaries.Remove(resourceDictionary);
                    break;
                }
            }

            switch (theme)
            {
                case Theme.Pink:
                    mergedDictionaries.Add(new PinkTheme());
                    break;

                case Theme.Orange:
                default:
                    mergedDictionaries.Add(new OrangeTheme());
                    break;
            }
        }
    }
}
