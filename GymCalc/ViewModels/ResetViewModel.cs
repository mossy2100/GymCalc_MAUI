using System.Runtime.CompilerServices;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Services;

namespace GymCalc.ViewModels;

public class ResetViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    // Dependencies.

    private readonly DatabaseHelperService _databaseHelperService;

    // ---------------------------------------------------------------------------------------------
    // Commands.

    public ICommand CancelCommand { get; init; }

    public ICommand ResetCommand { get; init; }

    // ---------------------------------------------------------------------------------------------
    // Bindable properties.

    private string _gymObjectTypeName;

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set => SetProperty(ref _gymObjectTypeName, value);
    }

    private string _title;

    public string Title
    {
        get => _title;

        set => SetProperty(ref _title, value);
    }

    private string _resetMessage;

    public string ResetMessage
    {
        get => _resetMessage;

        set => SetProperty(ref _resetMessage, value);
    }

    // ---------------------------------------------------------------------------------------------

    public ResetViewModel(DatabaseHelperService databaseHelperService)
    {
        _databaseHelperService = databaseHelperService;

        // Commands.
        CancelCommand = new AsyncCommand(Cancel);
        ResetCommand = new AsyncCommand(ResetGymObjects);
    }

    // ---------------------------------------------------------------------------------------------
    // Event handlers.

    /// <inheritdoc />
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(GymObjectTypeName):
                Title = $"Reset {GymObjectTypeName}s";
                ResetMessage = $"WARNING: This will remove all {GymObjectTypeName.ToLower()}s from "
                    + "the database and restore the defaults. Are you sure you want to do this?";
                break;
        }
    }

    // ---------------------------------------------------------------------------------------------
    // Command methods.

    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task ResetGymObjects()
    {
        var repo = _databaseHelperService.GetRepo(GymObjectTypeName);

        await repo.DeleteAll();
        await repo.InsertDefaults();

        await Shell.Current.GoToAsync("..");
    }
}
