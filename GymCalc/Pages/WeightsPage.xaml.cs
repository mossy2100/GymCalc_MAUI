using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class WeightsPage : ContentPage
{
    /// <summary>Reference to the viewmodel.</summary>
    private readonly WeightsViewModel _model;

    /// <summary>Constructor.</summary>
    /// <param name="weightsViewModel">The viewmodel to bind to.</param>
    public WeightsPage(WeightsViewModel weightsViewModel)
    {
        // Keep references to dependencies.
        _model = weightsViewModel;

        // Initialize.
        InitializeComponent();
        BindingContext = _model;
    }
}
