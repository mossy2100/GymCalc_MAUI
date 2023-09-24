using System.ComponentModel;
using System.Runtime.CompilerServices;
using GymCalc.Constants;
using GymCalc.Data;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
public partial class ResetPage : ContentPage
{
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

    public ResetPage(BarRepository barRepo, PlateRepository plateRepo, DumbbellRepository dbRepo,
        KettlebellRepository kbRepo)
    {
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
        await AppShell.GoToList(GymObjectTypeName, true);
    }

    private async void ResetButton_OnClicked(object sender, EventArgs e)
    {
        GymObjectRepository repo = GymObjectTypeName switch
        {
            GymObjectType.Bar => _barRepo,
            GymObjectType.Plate => _plateRepo,
            GymObjectType.Dumbbell => _dbRepo,
            GymObjectType.Kettlebell => _kbRepo,
            _ => throw new InvalidEnumArgumentException("Invalid object type."),
        };

        await repo.DeleteAll();
        await repo.InsertDefaults();

        await AppShell.GoToList(GymObjectTypeName, false);
    }
}
