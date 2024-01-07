using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class WeightsPage : ContentPage
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="viewModel">The viewmodel to bind to.</param>
    public WeightsPage(WeightsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
