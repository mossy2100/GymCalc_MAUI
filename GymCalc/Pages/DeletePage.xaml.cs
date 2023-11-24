using System.Runtime.CompilerServices;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
[QueryProperty(nameof(GymObjectId), "id")]
public partial class DeletePage : ContentPage
{
    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Reference to the viewmodel.
    /// </summary>
    private readonly DeleteViewModel _model;
    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// The id of the GymObject we want to delete.
    /// </summary>
    private int _gymObjectId;

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// The name of the GymObject type (e.g. "Bar").
    /// </summary>
    private string? _gymObjectTypeName;

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="model"></param>
    public DeletePage(DeleteViewModel model)
    {
        InitializeComponent();

        _model = model;
        BindingContext = model;

        // Workaround for issue with Back button label.
        // <see href="https://github.com/dotnet/maui/issues/8335" />
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    public string? GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set
        {
            _gymObjectTypeName = value;
            OnPropertyChanged();
        }
    }

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

    /// <inheritdoc/>
    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(GymObjectTypeName):
                Title = $"Delete {GymObjectTypeName}";
                _model.Initialize(GymObjectTypeName, GymObjectId);
                break;

            case nameof(GymObjectId):
                _model.Initialize(GymObjectTypeName, GymObjectId);
                break;
        }
    }
}
