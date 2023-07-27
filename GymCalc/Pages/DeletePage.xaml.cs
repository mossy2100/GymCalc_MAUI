using System.Runtime.CompilerServices;
using GymCalc.Constants;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
[QueryProperty(nameof(GymObjectId), "id")]
public partial class DeletePage : ContentPage
{
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

    public DeletePage()
    {
        InitializeComponent();
        BindingContext = this;

        // Workaround for issue with Back button label.
        // <see href="https://github.com/dotnet/maui/issues/8335" />
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior() { IsVisible = false });
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
                gymObject = await BarRepository.GetInstance().Get(GymObjectId);
                break;

            case GymObjectType.Plate:
                gymObject = await PlateRepository.GetInstance().Get(GymObjectId);
                break;

            case GymObjectType.Dumbbell:
                gymObject = await DumbbellRepository.GetInstance().Get(GymObjectId);
                break;

            case GymObjectType.Kettlebell:
                gymObject = await KettlebellRepository.GetInstance().Get(GymObjectId);
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
        await AppShell.GoToList(GymObjectTypeName, true);
    }

    private async void DeleteButton_OnClicked(object sender, EventArgs e)
    {
        var db = Database.GetConnection();

        switch (GymObjectTypeName)
        {
            case GymObjectType.Bar:
                await db.DeleteAsync<Bar>(GymObjectId);
                break;

            case GymObjectType.Plate:
                await db.DeleteAsync<Plate>(GymObjectId);
                break;

            case GymObjectType.Dumbbell:
                await db.DeleteAsync<Dumbbell>(GymObjectId);
                break;

            case GymObjectType.Kettlebell:
                await db.DeleteAsync<Kettlebell>(GymObjectId);
                break;
        }

        await AppShell.GoToList(GymObjectTypeName, true);
    }
}
