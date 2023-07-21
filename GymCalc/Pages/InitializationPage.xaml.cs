using GymCalc.Data;

namespace GymCalc.Pages;

public partial class InitializationPage : ContentPage
{
    public InitializationPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        await Database.Initialize();
        await Shell.Current.GoToAsync("//calculator");
    }
}
