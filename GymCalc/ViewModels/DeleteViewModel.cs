using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Data;
using GymCalc.Models;

namespace GymCalc.ViewModels;

public class DeleteViewModel : BaseViewModel
{
    // ---------------------------------------------------------------------------------------------
    // Dependencies.

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly BarbellRepository _barbellRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    // ---------------------------------------------------------------------------------------------
    // Fields

    /// <summary>
    /// Message to the user confirming the deletion.
    /// </summary>
    private string? _confirmDeletionMessage;

    /// <summary>
    /// The gym object.
    /// </summary>
    private GymObject? _gymObject;

    /// <summary>
    /// Page title.
    /// </summary>
    private string? _title;

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="barRepo"></param>
    /// <param name="plateRepo"></param>
    /// <param name="barbellRepo"></param>
    /// <param name="dumbbellRepo"></param>
    /// <param name="kettlebellRepo"></param>
    public DeleteViewModel(BarRepository barRepo, PlateRepository plateRepo,
        BarbellRepository barbellRepo, DumbbellRepository dumbbellRepo,
        KettlebellRepository kettlebellRepo)
    {
        // Dependencies.
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _barbellRepo = barbellRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;

        // Commands.
        CancelCommand = new AsyncCommand(Cancel);
        DeleteItemCommand = new AsyncCommand(DeleteItem);
    }

    // ---------------------------------------------------------------------------------------------
    // Commands.

    public ICommand CancelCommand { get; init; }

    public ICommand DeleteItemCommand { get; init; }

    public string? Title
    {
        get => _title;

        set => SetProperty(ref _title, value);
    }

    public string? ConfirmDeletionMessage
    {
        get => _confirmDeletionMessage;

        set => SetProperty(ref _confirmDeletionMessage, value);
    }

    // ---------------------------------------------------------------------------------------------
    // Commands.

    /// <summary>
    /// Go back to the list page, showing items of the current type.
    /// </summary>
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Delete the item from the database, then go back to the list page.
    /// </summary>
    private async Task DeleteItem()
    {
        switch (_gymObject)
        {
            case Bar:
                await _barRepo.Delete(_gymObject.Id);
                break;

            case Plate:
                await _plateRepo.Delete(_gymObject.Id);
                break;

            case Barbell:
                await _barbellRepo.Delete(_gymObject.Id);
                break;

            case Dumbbell:
                await _dumbbellRepo.Delete(_gymObject.Id);
                break;

            case Kettlebell:
                await _kettlebellRepo.Delete(_gymObject.Id);
                break;
        }

        await Shell.Current.GoToAsync("..");
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Initialize the view once the parameters have been set.
    /// </summary>
    /// <param name="gymObjectTypeName"></param>
    /// <param name="gymObjectId"></param>
    /// <returns>If the initialization completed ok.</returns>
    internal async Task Initialize(string? gymObjectTypeName, int? gymObjectId)
    {
        // Don't do anything unless both parameters have been set.
        if (string.IsNullOrEmpty(gymObjectTypeName) || gymObjectId == null)
        {
            return;
        }

        Title = $"Delete {gymObjectTypeName}";

        switch (gymObjectTypeName)
        {
            case nameof(Bar):
                _gymObject = await _barRepo.LoadOne(gymObjectId.Value);
                break;

            case nameof(Plate):
                _gymObject = await _plateRepo.LoadOne(gymObjectId.Value);
                break;

            case nameof(Barbell):
                _gymObject = await _barbellRepo.LoadOne(gymObjectId.Value);
                break;

            case nameof(Dumbbell):
                _gymObject = await _dumbbellRepo.LoadOne(gymObjectId.Value);
                break;

            case nameof(Kettlebell):
                _gymObject = await _kettlebellRepo.LoadOne(gymObjectId.Value);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(gymObjectTypeName),
                    $"Invalid gym object type '{gymObjectTypeName}'.");
        }

        if (_gymObject == null)
        {
            throw new ArgumentOutOfRangeException(nameof(gymObjectId),
                $"Invalid gym object id ({gymObjectId}).");
        }

        ConfirmDeletionMessage =
            $"Are you sure you want to delete the {_gymObject.Weight} {_gymObject.Units} {gymObjectTypeName.ToLower()}?";
    }
}
