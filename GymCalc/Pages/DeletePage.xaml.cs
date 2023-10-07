using System.Runtime.CompilerServices;
using GymCalc.Constants;
using GymCalc.Data;
using GymCalc.Models;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
[QueryProperty(nameof(GymObjectId), "id")]
public partial class DeletePage : ContentPage
{
    private readonly Database _database;

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly DumbbellRepository _dbRepo;

    private readonly KettlebellRepository _kbRepo;

    private string _gymObjectTypeName;

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set
        {
            _gymObjectTypeName = value;
            OnPropertyChanged();
        }
    }

    private int _gymObjectId;

    public int GymObjectId
    {
        get => _gymObjectId;

        set
        {
            _gymObjectId = value;
            OnPropertyChanged();
        }
    }

    public DeletePage(Database database, BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dbRepo, KettlebellRepository kbRepo)
    {
        _database = database;
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dbRepo = dbRepo;
        _kbRepo = kbRepo;

        InitializeComponent();
        BindingContext = this;

        // Workaround for issue with Back button label.
        // <see href="https://github.com/dotnet/maui/issues/8335" />
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    /// <inheritdoc />
    protected override async void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(GymObjectTypeName):
                Title = $"Delete {GymObjectTypeName}";
                await InitializeForm();
                break;

            case nameof(GymObjectId):
                await InitializeForm();
                break;
        }
    }

    private async Task InitializeForm()
    {
        if (string.IsNullOrEmpty(GymObjectTypeName) || GymObjectId == 0)
        {
            return;
        }

        GymObject gymObject;

        switch (GymObjectTypeName)
        {
            case GymObjectType.Bar:
                gymObject = await _barRepo.Get(GymObjectId);
                break;

            case GymObjectType.Plate:
                gymObject = await _plateRepo.Get(GymObjectId);
                break;

            case GymObjectType.Dumbbell:
                gymObject = await _dbRepo.Get(GymObjectId);
                break;

            case GymObjectType.Kettlebell:
                gymObject = await _kbRepo.Get(GymObjectId);
                break;

            default:
                return;
        }

        if (gymObject == null)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        DeleteMessage.Text =
            $"Are you sure you want to delete the {gymObject.Weight} {gymObject.Units} {GymObjectTypeName.ToLower()}?";
    }

    private async void CancelButton_OnClicked(object sender, EventArgs e)
    {
        await AppShell.GoToList(GymObjectTypeName);
    }

    private async void DeleteButton_OnClicked(object sender, EventArgs e)
    {
        var conn = _database.Connection;

        switch (GymObjectTypeName)
        {
            case GymObjectType.Bar:
                await conn.DeleteAsync<Bar>(GymObjectId);
                break;

            case GymObjectType.Plate:
                await conn.DeleteAsync<Plate>(GymObjectId);
                break;

            case GymObjectType.Dumbbell:
                await conn.DeleteAsync<Dumbbell>(GymObjectId);
                break;

            case GymObjectType.Kettlebell:
                await conn.DeleteAsync<Kettlebell>(GymObjectId);
                break;
        }

        await AppShell.GoToList(GymObjectTypeName);
    }
}
