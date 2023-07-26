using System.ComponentModel;
using System.Runtime.CompilerServices;
using GymCalc.Constants;
using GymCalc.Data.Repositories;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
public partial class ResetPage : ContentPage
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

    public ResetPage()
    {
        InitializeComponent();
        BindingContext = this;
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
        await Shell.Current.GoToAsync("..");
    }

    private async void ResetButton_OnClicked(object sender, EventArgs e)
    {
        GymObjectRepository repo = GymObjectTypeName switch
        {
            GymObjectType.Bar => BarRepository.GetInstance(),
            GymObjectType.Plate => PlateRepository.GetInstance(),
            GymObjectType.Dumbbell => DumbbellRepository.GetInstance(),
            GymObjectType.Kettlebell => KettlebellRepository.GetInstance(),
            _ => throw new InvalidEnumArgumentException("Invalid object type."),
        };

        await repo.DeleteAll();
        await repo.InsertDefaults();

        await Shell.Current.GoToAsync("..");
    }
}
