using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Pages;

namespace GymCalc;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        BindingContext = this;
        RegisterRoutes();
    }

    public ICommand GoToRouteCommand => new AsyncCommand<string>(GoToRoute);

    public ICommand GoToListCommand => new AsyncCommand<string>(GoToList);

    public ICommand GoToInstructionsCommand =>
        new AsyncCommand(async () => await GoToHtml("Instructions", "/Instructions"));

    public ICommand GoToAboutCommand =>
        new AsyncCommand(async () => await GoToHtml("About GymCalc", "/About"));

    /// <summary>
    /// Register routes for navigation pages.
    /// </summary>
    private static void RegisterRoutes()
    {
        // Routing.RegisterRoute("calculator", typeof(CalculatorPage));
        Routing.RegisterRoute("edit", typeof(EditPage));
        Routing.RegisterRoute("delete", typeof(DeletePage));
        Routing.RegisterRoute("reset", typeof(ResetPage));
        // Routing.RegisterRoute("settings", typeof(SettingsPage));
    }

    #region Command methods

    internal static async Task GoToRoute(string route)
    {
        Current.FlyoutIsPresented = false;
        await Current.GoToAsync($"//{route}");
    }

    internal static async Task GoToList(string gymObjectTypeName)
    {
        Current.FlyoutIsPresented = false;
        await Current.GoToAsync("//list", new Dictionary<string, object>
        {
            { "type", gymObjectTypeName }
        });
    }

    internal static async Task GoToHtml(string title, string route)
    {
        Current.FlyoutIsPresented = false;
        await Current.GoToAsync("//html", new Dictionary<string, object>
        {
            { "title", title },
            { "route", route }
        });
    }

    #endregion Command methods
}
