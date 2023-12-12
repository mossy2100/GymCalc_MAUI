using System.Runtime.CompilerServices;
using GymCalc.Services;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(Title), "title")]
[QueryProperty(nameof(Route), "route")]
public partial class HtmlPage : ContentPage
{
    private readonly HtmlUpdaterService _htmlUpdaterService;

    /// <summary>Reference to the viewmodel.</summary>
    private readonly HtmlViewModel _model;

    // The route to the Blazor page.
    private string? _route;

    public HtmlPage(HtmlViewModel model, HtmlUpdaterService htmlUpdaterService)
    {
        _model = model;
        _htmlUpdaterService = htmlUpdaterService;

        InitializeComponent();
        BindingContext = _model;

        // Events.
        Application.Current!.RequestedThemeChanged += OnRequestedThemeChanged;

        // Set the root component parameters. This can only be done once (init only).
        // Because RequestedTheme is not set at the start (it's Unspecified), I'm using a hack to
        // detect the current theme using AppThemeBinding and the BackgroundColor property of the
        // BlazorWebView element in the XAML.
        AppTheme theme = BlazorWebView.BackgroundColor.Equals(Colors.White)
            ? AppTheme.Light
            : AppTheme.Dark;
        RootComponent.Parameters = new Dictionary<string, object?> { { "Theme", theme } };
    }

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
