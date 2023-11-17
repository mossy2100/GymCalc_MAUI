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

    public ICommand ListCommand => new AsyncCommand<string>(GoToList);

    public ICommand InstructionsCommand =>
        new AsyncCommand(async () => await GoToHtml("Instructions", "/Instructions"));

    public ICommand AboutCommand =>
        new AsyncCommand(async () => await GoToHtml("About GymCalc", "/About"));

    /// <summary>
    /// Register routes for navigation pages.
    /// </summary>
    private static void RegisterRoutes()
    {
        Routing.RegisterRoute("edit", typeof(EditPage));
        Routing.RegisterRoute("delete", typeof(DeletePage));
        Routing.RegisterRoute("reset", typeof(ResetPage));
    }

    #region Command methods

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
