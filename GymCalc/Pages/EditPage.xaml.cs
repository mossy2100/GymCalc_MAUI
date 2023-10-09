using System.Runtime.CompilerServices;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(Operation), "op")]
[QueryProperty(nameof(GymObjectTypeName), "type")]
[QueryProperty(nameof(GymObjectId), "id")]
public partial class EditPage : ContentPage
{
    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Reference to the view model.
    /// </summary>
    private EditViewModel _model;

    // ---------------------------------------------------------------------------------------------
    // Page parameters.

    private string _operation;

    public string Operation
    {
        get => _operation;

        set
        {
            _operation = value;
            OnPropertyChanged();
        }
    }

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

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="model"></param>
    public EditPage(EditViewModel model)
    {
        InitializeComponent();
        _model = model;
        BindingContext = _model;

        // Workaround for issue with Back button label.
        // <see href="https://github.com/dotnet/maui/issues/8335" />
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    // ---------------------------------------------------------------------------------------------
    // Event handlers.

    /// <inheritdoc />
    protected override async void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(Operation):
            case nameof(GymObjectTypeName):
            case nameof(GymObjectId):
                await _model.Initialize(Operation, GymObjectTypeName, GymObjectId);
                break;
        }
    }
}
