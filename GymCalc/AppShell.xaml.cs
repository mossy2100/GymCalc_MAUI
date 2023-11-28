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

    public ICommand GoToPageCommand => new AsyncCommand<string>(GoToPage);

    /// <summary>Register routes for navigation (non-global) pages.</summary>
    private static void RegisterRoutes()
    {
        Routing.RegisterRoute("list", typeof(ListPage));
        Routing.RegisterRoute("edit", typeof(EditPage));
    }

    private static async Task GoToPage(string? pageName)
    {
        Current.FlyoutIsPresented = false;

        string? route = null;
        Dictionary<string, object>? parameters = null;

        switch (pageName)
        {
            case "Calculator":
                route = "//calculator";
                break;

            case "Bars":
                route = "//list";
                parameters = new Dictionary<string, object> { { "type", "Bar" } };
                break;

            case "Plates":
                route = "//list";
                parameters = new Dictionary<string, object> { { "type", "Plate" } };
                break;

            case "Dumbbells":
                route = "//list";
                parameters = new Dictionary<string, object> { { "type", "Dumbbell" } };
                break;

            case "Kettlebells":
                route = "//list";
                parameters = new Dictionary<string, object> { { "type", "Kettlebell" } };
                break;

            case "Instructions":
                route = "//html";
                parameters = new Dictionary<string, object>
                    { { "title", "Instructions" }, { "route", "/Instructions" } };
                break;

            case "About":
                route = "//html";
                parameters = new Dictionary<string, object>
                    { { "title", "About GymCalc" }, { "route", "/About" } };
                break;

            case "Settings":
                route = "//settings";
                break;
        }

        // If a route is specified, go to that page.
        if (route != null)
        {
            if (parameters == null)
            {
                await Current.GoToAsync(route);
            }
            else
            {
                await Current.GoToAsync(route, parameters);
            }
        }
    }
}
