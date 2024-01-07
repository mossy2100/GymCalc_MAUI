using AsyncAwaitBestPractices.MVVM;

namespace GymCalc.ViewModels;

public class ManualViewModel : BaseViewModel
{
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public ManualViewModel()
    {
        // Create commands.
        GoToInstructionsPageCommand = new AsyncCommand(GoToInstructionsPage);
        GoToTrainingPageCommand = new AsyncCommand(GoToTrainingPage);
        GoToAboutPageCommand = new AsyncCommand(GoToAboutPage);
    }

    #endregion Constructors

    #region Commands

    // ---------------------------------------------------------------------------------------------
    public ICommand GoToInstructionsPageCommand { get; init; }

    private async Task GoToInstructionsPage()
    {
        await Shell.Current.GoToAsync("html", new Dictionary<string, object>
        {
            { "title", "Instructions" },
            { "route", "/instructions" }
        });
    }

    // ---------------------------------------------------------------------------------------------
    public ICommand GoToTrainingPageCommand { get; init; }

    private async Task GoToTrainingPage()
    {
        await Shell.Current.GoToAsync("html", new Dictionary<string, object>
        {
            { "title", "Training Methods" },
            { "route", "/training" }
        });
    }

    // ---------------------------------------------------------------------------------------------
    public ICommand GoToAboutPageCommand { get; init; }

    private async Task GoToAboutPage()
    {
        await Shell.Current.GoToAsync("html", new Dictionary<string, object>
        {
            { "title", "About GymCalc" },
            { "route", "/about" }
        });
    }

    #endregion Commands
}
