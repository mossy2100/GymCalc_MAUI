using System.Runtime.CompilerServices;
using GymCalc.Services;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(Title), "title")]
[QueryProperty(nameof(Route), "route")]
public partial class HtmlPage : ContentPage
{
    /// <summary>
    /// Reference to the dependency.
    /// </summary>
    private readonly HtmlUpdaterService _htmlUpdaterService;

    /// <summary>
    /// Reference to the viewmodel.
    /// </summary>
    private readonly HtmlViewModel _model;

    /// <summary>
    /// The route to the Blazor page (backing field).
    /// </summary>
    private string? _route;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="model">Reference to the viewmodel.</param>
    /// <param name="htmlUpdaterService">Reference to the HtmlUpdaterService.</param>
    public HtmlPage(HtmlViewModel model, HtmlUpdaterService htmlUpdaterService)
    {
        // Keep references to dependencies.
        _model = model;
        _htmlUpdaterService = htmlUpdaterService;

        // Initialize.
        InitializeComponent();
        BindingContext = _model;

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
                // Copy the title to the model.
                _model.Title = Title;
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
}
