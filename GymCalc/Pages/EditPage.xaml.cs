using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(Operation), "op")]
[QueryProperty(nameof(GymObjectTypeName), "type")]
[QueryProperty(nameof(GymObjectId), "id")]
public partial class EditPage : ContentPage
{
    #region Fields

    /// <summary>Reference to the viewmodel.</summary>
    private readonly EditViewModel _viewModel;

    private string? _operation;

    private int _gymObjectId;

    private string? _gymObjectTypeName;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="viewModel"></param>
    public EditPage(EditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    #endregion Constructor

    #region Properties

    public string? Operation
    {
        get => _operation;

        set
        {
            _operation = value;
            OnPropertyChanged();
        }
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

    #endregion Properties

    #region Events

    /// <inheritdoc/>
    protected override async void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(Operation):
            case nameof(GymObjectTypeName):
            case nameof(GymObjectId):
                await _viewModel.Initialize(Operation, GymObjectTypeName, GymObjectId);
                break;
        }
    }

    #endregion Events
}
