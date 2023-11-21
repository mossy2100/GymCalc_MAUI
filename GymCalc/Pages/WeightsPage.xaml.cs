using GymCalc.ViewModels;

namespace GymCalc.Pages;

public partial class WeightsPage : ContentPage
{
    /// <summary>Reference to the viewmodel.</summary>
    public WeightsViewModel Model { get; }

    /// <summary>Constructor.</summary>
    /// <param name="weightsViewModel">The viewmodel to bind to.</param>
    public WeightsPage(WeightsViewModel weightsViewModel)
    {
        // Keep references to dependencies.
        Model = weightsViewModel;

        // Initialize.
        InitializeComponent();
        BindingContext = Model;
    }
}
