using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
public partial class ListPage : ContentPage
{
    private readonly ListViewModel _model;

    private string _gymObjectTypeName;

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set
        {
            _model.GymObjectTypeName = value;
            _isGymObjectTypeNameSet = true;
        }
    }

    private bool _isGymObjectTypeNameSet;

    public ListPage(ListViewModel listViewModel)
    {
        // Keep references to dependencies.
        _model = listViewModel;

        // Initialize.
        InitializeComponent();
        BindingContext = _model;

        // Event handlers.
        SizeChanged += OnSizeChanged;
    }

    #region Event handlers

    private async void OnSizeChanged(object sender, EventArgs e)
    {
        await _model.DisplayList();
    }

    #endregion Event handlers
}
