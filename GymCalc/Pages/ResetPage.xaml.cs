using System.ComponentModel;
using System.Runtime.CompilerServices;
using GymCalc.Constants;
using GymCalc.Data;
using GymCalc.Services;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
public partial class ResetPage : ContentPage
{
    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly DumbbellRepository _dumbbellRepo;

    private readonly KettlebellRepository _kettlebellRepo;

    private readonly DatabaseHelperService _databaseHelperService;

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

    public ResetPage(BarRepository barRepo, PlateRepository plateRepo, DumbbellRepository dumbbellRepo,
        KettlebellRepository kettlebellRepo, DatabaseHelperService databaseHelperService)
    {
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dumbbellRepo = dumbbellRepo;
        _kettlebellRepo = kettlebellRepo;
        _databaseHelperService = databaseHelperService;

        InitializeComponent();
        BindingContext = this;

        // Workaround for issue with Back button label.
        // <see href="https://github.com/dotnet/maui/issues/8335" />
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(GymObjectTypeName):
                Title = $"Reset {GymObjectTypeName}s";
                InitializeForm();
                break;
        }
    }

    private void InitializeForm()
    {
        ResetMessage.Text = $"WARNING: This will remove all {GymObjectTypeName.ToLower()}s from the"
            + " database and restore the defaults. Are you sure you want to do this?";
    }

    private async void CancelButton_OnClicked(object sender, EventArgs e)
    {
        await AppShell.GoToList(GymObjectTypeName);
    }

    private async void ResetButton_OnClicked(object sender, EventArgs e)
    {
        var repo = _databaseHelperService.GetRepo(GymObjectTypeName);

        await repo.DeleteAll();
        await repo.InsertDefaults();

        await AppShell.GoToList(GymObjectTypeName);
    }
}
