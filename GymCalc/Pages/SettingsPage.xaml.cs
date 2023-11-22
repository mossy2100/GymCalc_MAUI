using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class SettingsPage : ContentPage
{
    /// <summary>Reference to the viewmodel.</summary>
    private readonly SettingsViewModel _model;

    public SettingsPage(SettingsViewModel model)
    {
        _model = model;

        InitializeComponent();
        BindingContext = _model;
    }
}
