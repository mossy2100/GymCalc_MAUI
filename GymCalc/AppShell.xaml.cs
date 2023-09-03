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

    internal static async Task GoToList(string gymObjectTypeName, bool editMode)
    {
        await Current.GoToAsync($"//list", new Dictionary<string, object>
        {
            { "type", gymObjectTypeName },
            { "editMode", editMode },
        });
    }

    private async void Bars_OnClick(object sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToList("Bar", false);
    }

    private async void Plates_OnClick(object sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToList("Plate", false);
    }

    private async void Dumbbells_OnClick(object sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToList("Dumbbell", false);
    }

    private async void Kettlebells_OnClick(object sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToList("Kettlebell", false);
    }
}
