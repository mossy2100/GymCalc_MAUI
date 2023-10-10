using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class SettingsPage : ContentPage
{
    private SettingsViewModel _model;

    public SettingsPage(SettingsViewModel model)
    {
        _model = model;

        InitializeComponent();
        BindingContext = _model;
    }
}
