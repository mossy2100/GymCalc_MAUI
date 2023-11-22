using System.Runtime.CompilerServices;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Data;

namespace GymCalc.ViewModels;

public class ResetViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    // Dependencies.

    private readonly Database _database;

    // ---------------------------------------------------------------------------------------------
    // Bindable properties.

    private string _gymObjectTypeName;

    private string _resetMessage;

    private string _title;

    // ---------------------------------------------------------------------------------------------

    public ResetViewModel(Database database)
    {
        _database = database;

        // Commands.
        CancelCommand = new AsyncCommand(Cancel);
        ResetCommand = new AsyncCommand(ResetGymObjects);
    }

    // ---------------------------------------------------------------------------------------------
    // Commands.

    public ICommand CancelCommand { get; init; }

    public ICommand ResetCommand { get; init; }

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set => SetProperty(ref _gymObjectTypeName, value);
    }

    public string Title
    {
        get => _title;

        set => SetProperty(ref _title, value);
    }

    public string ResetMessage
    {
        get => _resetMessage;

        set => SetProperty(ref _resetMessage, value);
    }

    // ---------------------------------------------------------------------------------------------
    // Event handlers.

    /// <inheritdoc/>
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
        var repo = _database.GetRepo(GymObjectTypeName);
        await repo.DeleteAll();
        await repo.InsertDefaults();
        await Shell.Current.GoToAsync("..");
    }
}
