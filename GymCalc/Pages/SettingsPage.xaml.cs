using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class SettingsPage : ContentPage
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="model">The viewmodel to bind to.</param>
    public SettingsPage(SettingsViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
