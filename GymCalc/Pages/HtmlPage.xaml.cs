using GymCalc.Services;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(Title), "title")]
[QueryProperty(nameof(Route), "route")]
public partial class HtmlPage : ContentPage
{
    #region Fields

    /// <summary>
    /// Reference to the viewmodel.
    /// </summary>
    private readonly HtmlViewModel _viewModel;

    /// <summary>
    /// Reference to the service dependency.
    /// </summary>
    private readonly HtmlUpdaterService _htmlUpdaterService;

    /// <summary>
    /// The route to the Blazor page (backing field).
    /// </summary>
    private string? _route;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="viewModel">Reference to the viewmodel.</param>
    /// <param name="htmlUpdaterService">Reference to the HtmlUpdaterService.</param>
    public HtmlPage(HtmlViewModel viewModel, HtmlUpdaterService htmlUpdaterService)
    {
        // Keep references to dependencies.
        _viewModel = viewModel;
        _htmlUpdaterService = htmlUpdaterService;

        // Initialize.
        InitializeComponent();
        BindingContext = _viewModel;
        Title = "Manual";

        // Events.
        Application.Current!.RequestedThemeChanged += OnRequestedThemeChanged;

        // Set the root component parameters. This can only be done once (init only).
        // Because RequestedTheme is not set at the start (it's Unspecified), I'm using a hack to
        // detect the current theme by inspecting the BackgroundColor property of the BlazorWebView
        // element in the XAML. This property is set using AppThemeBinding and therefore reflects
        // the current theme.
        AppTheme theme = BlazorWebView.BackgroundColor.Equals(Colors.White)
            ? AppTheme.Light
            : AppTheme.Dark;
        RootComponent.Parameters = new Dictionary<string, object?>
        {
            { "Theme", theme }
        };
    }

    #endregion Constructor

    #region Properties

    /// <summary>
    /// The route to the Blazor page (property).
    /// </summary>
    public string? Route
    {
        get => _route;

        set
        {
            if (_route != value)
            {
                _route = value;
                OnPropertyChanged();
            }
        }
    }

    #endregion Properties

    #region Events

    /// <summary>
    /// Event handler for when the theme changes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        _htmlUpdaterService.UpdateTheme(Application.Current!.RequestedTheme);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(Title):
                // Copy the title to the viewmodel.
                _viewModel.Title = Title;
                break;

            case nameof(Route):
                // Navigate to the specified route if necessary.
                // We can't directly access methods on the component, so use the service to transfer
                // the route parameter into the component.
                if (!string.IsNullOrEmpty(Route))
                {
                    _htmlUpdaterService.UpdateRoute(Route);
                }
                break;
        }
    }

    #endregion Events
}
