using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class WeightsPage : ContentPage
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="model">The viewmodel to bind to.</param>
    public WeightsPage(WeightsViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
