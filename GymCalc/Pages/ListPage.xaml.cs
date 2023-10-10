using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
public partial class ListPage : ContentPage
{
    /// <summary>
    /// Reference to the viewmodel.
    /// </summary>
    private readonly ListViewModel _model;

    public ListViewModel Model => _model;

    /// <summary>
    /// The gym object type name. This is passed as a parameter to the page and determines what
    /// objects to display in the list.
    /// As this page is a singleton, whenever the page is navigated to, the list is refreshed with
    /// gym objects of the specified type.
    /// </summary>
    private string _gymObjectTypeName;

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set
        {
            _gymObjectTypeName = value;

            // Copy the value to the model.
            _model.GymObjectTypeName = value;
        }
    }

    /// <summary>
    /// Constructon
    /// </summary>
    /// <param name="listViewModel"></param>
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

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _model.DisplayList();
    }

    /// <summary>
    /// Re-render the list if the page orientation changes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSizeChanged(object sender, EventArgs e)
    {
        await _model.DisplayList();
    }
}
