using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class ManualPage : ContentPage
{
    public ManualPage(ManualViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
