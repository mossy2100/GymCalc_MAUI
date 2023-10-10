using System.Runtime.CompilerServices;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
public partial class ResetPage : ContentPage
{
    private ResetViewModel _model;

    private string _gymObjectTypeName;

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set
        {
            if (value != _gymObjectTypeName)
            {
                _gymObjectTypeName = value;
                OnPropertyChanged();
            }
        }
    }

    public ResetPage(ResetViewModel model)
    {
        _model = model;

        InitializeComponent();
        BindingContext = _model;

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
                // Copy value to the viewmodel.
                _model.GymObjectTypeName = GymObjectTypeName;
                break;
        }
    }
}
