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

    public ICommand GoToHtmlCommand =>
        new AsyncCommand<string>(async parameters => await GoToHtml(parameters));

    private static async Task GoToHtml(string commandParameters)
    {
        var parameters = commandParameters.Split('|');
        if (parameters.Length != 2)
        {
            throw new ArgumentException("Invalid number of command parameters. There should be 2: the page title and the filename, separated by a vertical bar character (|).");
        }

        Current.FlyoutIsPresented = false;
        await Current.GoToAsync("//html", new Dictionary<string, object>
        {
            { "title", parameters[0] },
            { "fileName", parameters[1] },
        });
    }
    #endregion Commands
}
