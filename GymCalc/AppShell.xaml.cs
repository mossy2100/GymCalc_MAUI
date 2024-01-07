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

    /// <summary>Register routes for navigation pages.</summary>
    private static void RegisterRoutes()
    {
        Routing.RegisterRoute("results", typeof(ResultsPage));
        Routing.RegisterRoute("list", typeof(ListPage));
        Routing.RegisterRoute("edit", typeof(EditPage));
        Routing.RegisterRoute("html", typeof(HtmlPage));
    }
}
