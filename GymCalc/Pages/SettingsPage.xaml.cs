using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class SettingsPage : ContentPage
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="viewModel">The viewmodel to bind to.</param>
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
