using GymCalc.Pages;

namespace GymCalc;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation pages.
        Routing.RegisterRoute("edit", typeof(EditPage));
        Routing.RegisterRoute("delete", typeof(DeletePage));
        Routing.RegisterRoute("reset", typeof(ResetPage));
    }

    private async Task GoToList(string gymObjectTypeName)
    {
        FlyoutIsPresented = false;
        await Current.GoToAsync($"//list?type={gymObjectTypeName}");
    }

    private async void Bars_OnClick(object sender, EventArgs e)
    {
        await GoToList("Bar");
    }

    private async void Plates_OnClick(object sender, EventArgs e)
    {
        await GoToList("Plate");
    }

    private async void Dumbbells_OnClick(object sender, EventArgs e)
    {
        await GoToList("Dumbbell");
    }

    private async void Kettlebells_OnClick(object sender, EventArgs e)
    {
        await GoToList("Kettlebell");
    }
}
