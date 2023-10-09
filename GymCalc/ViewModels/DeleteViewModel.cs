using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Constants;
using GymCalc.Data;
using GymCalc.Models;

namespace GymCalc.ViewModels;

public class DeleteViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    // Dependencies.

    private readonly Database _database;

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    // ---------------------------------------------------------------------------------------------
    // Commands.

    public ICommand CancelCommand { get; init; }

    public ICommand DeleteItemCommand { get; init; }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Message to the user confirming the deletion.
    /// </summary>
    private string _confirmDeletionMessage;

    public string ConfirmDeletionMessage
    {
        get => _confirmDeletionMessage;

        set => SetProperty(ref _confirmDeletionMessage, value);
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// The gym object.
    /// </summary>
    private GymObject _gymObject;

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="database"></param>
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="dumbbellRepo"></param>
    /// <param name="kettlebellRepo"></param>
    public DeleteViewModel(Database database, BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dumbbellRepo, KettlebellRepository kettlebellRepo)
    {
        // Dependencies.
        _database = database;
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;

        // Commands.
        CancelCommand = new AsyncCommand(Cancel);
        DeleteItemCommand = new AsyncCommand(DeleteItem);
    }

    // ---------------------------------------------------------------------------------------------
    // Commands.

    /// <summary>
    /// Go back to the list page, showing items of the current type.
    /// </summary>
    private async Task Cancel()
    {
        // await AppShell.GoToList(_gymObjectTypeName);
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Delete the item from the database, then go back to the list page.
    /// </summary>
    private async Task DeleteItem()
    {
        var conn = _database.Connection;

        switch (_gymObject)
        {
            case Bar:
                await _barRepo.Delete(_gymObject.Id);
                break;

            case Plate:
                await _plateRepo.Delete(_gymObject.Id);
                break;

            case Dumbbell:
                await _dumbbellRepo.Delete(_gymObject.Id);
                break;

            case Kettlebell:
                await _kettlebellRepo.Delete(_gymObject.Id);
                break;
        }

        // await AppShell.GoToList(_gymObjectTypeName);
        await Shell.Current.GoToAsync("..");
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Initialize the view once the parameters have been set.
    /// </summary>
    /// <param name="gymObjectTypeName"></param>
    /// <param name="gymObjectId"></param>
    /// <returns>If the initialization completed ok.</returns>
    internal async Task<bool> Initialize(string gymObjectTypeName, int gymObjectId)
    {
        if (string.IsNullOrEmpty(gymObjectTypeName) || gymObjectId == 0)
        {
            return false;
        }

        switch (gymObjectTypeName)
        {
            case GymObjectType.Bar:
                _gymObject = await _barRepo.Get(gymObjectId);
                break;

            case GymObjectType.Plate:
                _gymObject = await _plateRepo.Get(gymObjectId);
                break;

            case GymObjectType.Dumbbell:
                _gymObject = await _dumbbellRepo.Get(gymObjectId);
                break;

            case GymObjectType.Kettlebell:
                _gymObject = await _kettlebellRepo.Get(gymObjectId);
                break;

            default:
                return false;
        }

        if (_gymObject == null)
        {
            return false;
        }

        ConfirmDeletionMessage =
            $"Are you sure you want to delete the {_gymObject.Weight} {_gymObject.Units} {gymObjectTypeName.ToLower()}?";
        return true;
    }
}
