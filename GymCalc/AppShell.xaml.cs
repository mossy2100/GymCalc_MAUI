using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Pages;

namespace GymCalc;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
        BindingContext = this;
    }

    /// <summary>
    /// Register routes for navigation pages.
    /// </summary>
    private static void RegisterRoutes()
    {
        Routing.RegisterRoute("edit", typeof(EditPage));
        Routing.RegisterRoute("delete", typeof(DeletePage));
        Routing.RegisterRoute("reset", typeof(ResetPage));
    }

    #region Commands

    public ICommand GoToListCommand =>
        new AsyncCommand<string>(
            async gymObjectTypeName => await GoToList(gymObjectTypeName, false));

    internal static async Task GoToList(string gymObjectTypeName, bool editMode)
    {
        Current.FlyoutIsPresented = false;
        await Current.GoToAsync("//list", new Dictionary<string, object>
        {
            { "type", gymObjectTypeName },
            { "editMode", editMode },
        });
    }

    public ICommand GoToHtmlCommand(string title, string route)
    {
        return new AsyncCommand(async () =>
        {
            Current.FlyoutIsPresented = false;

            // Load the HtmlPage.
            await Current.GoToAsync("//html", new Dictionary<string, object>
            {
                { "title", title },
                { "route", route },
            });
        });
    }

    public ICommand GoToInstructionsPageCommand => GoToHtmlCommand("Instructions", "/Instructions");

    public ICommand GoToAboutPageCommand => GoToHtmlCommand("About GymCalc", "/About");

    #endregion Commands
}
