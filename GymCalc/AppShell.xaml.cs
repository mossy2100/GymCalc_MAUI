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
        Routing.RegisterRoute("results", typeof(ResultsPage));
        Routing.RegisterRoute("weights", typeof(WeightsPage));
        Routing.RegisterRoute("list", typeof(ListPage));
        Routing.RegisterRoute("edit", typeof(EditPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
        Routing.RegisterRoute("html", typeof(HtmlPage));
    }

    private static async Task GoToPage(string? pageName)
    {
        Current.FlyoutIsPresented = false;

        string? state;
        Dictionary<string, object>? parameters = null;

        switch (pageName)
        {
            case "instructions":
                state = "html";
                parameters = new Dictionary<string, object>
                {
                    { "title", "Instructions" },
                    { "route", "/Instructions" }
                };
                break;

            case "about":
                state = "html";
                parameters = new Dictionary<string, object>
                {
                    { "title", "About GymCalc" },
                    { "route", "/About" }
                };
                break;

            default:
                state = pageName;
                break;
        }

        // If a route is specified, go to that page.
        if (state != null)
        {
            if (parameters == null)
            {
                await Current.GoToAsync(state);
            }
            else
            {
                await Current.GoToAsync(state, parameters);
            }
        }
    }
}
