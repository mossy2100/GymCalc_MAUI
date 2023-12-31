using AsyncAwaitBestPractices.MVVM;

namespace GymCalc.ViewModels;

public class WeightsViewModel : BaseViewModel
{
    #region Constructor

    /// <summary>Constructor.</summary>
    public WeightsViewModel()
    {
        // Commands.
        ListGymObjectsCommand = new AsyncCommand<string>(ListGymObjects);
    }

    #endregion Constructor

    #region Commands

    // ---------------------------------------------------------------------------------------------
    /// <summary>Command to navigate to the list page for a gym object type.</summary>
    public ICommand ListGymObjectsCommand { get; init; }

    /// <summary>Navigate to the list page for a gym object type.</summary>
    /// <param name="gymObjectTypeName">The gym object type name.</param>
    private async Task ListGymObjects(string? gymObjectTypeName)
    {
        await Shell.Current.GoToAsync($"list?type={gymObjectTypeName}");
    }

    #endregion Commands
}
