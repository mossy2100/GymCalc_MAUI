using System.Runtime.CompilerServices;
using GymCalc.Services;

namespace GymCalc.Pages;

[QueryProperty(nameof(Title), "title")]
[QueryProperty(nameof(Route), "route")]
public partial class HtmlPage : ContentPage
{
    private readonly HtmlUpdaterService _htmlUpdaterService;
    // private const string _STYLES_CSS_LINK_ID = "styles-css";
    //
    // private const string _THEME_CSS_LINK_ID = "theme-css";

    private string _route;

    public string Route
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

    public HtmlPage(HtmlUpdaterService htmlUpdaterService)
    {
        _htmlUpdaterService = htmlUpdaterService;
        InitializeComponent();
        BindingContext = this;

        // // Inject CSS files.
        // InjectCss(_STYLES_CSS_LINK_ID, "css/styles.css");
        // InjectCss(_THEME_CSS_LINK_ID, GetThemeCssPath());
        //
        // // React to theme change.
        // Application.Current!.RequestedThemeChanged += OnRequestedThemeChanged;
    }

    // private void InjectCss(string elementId, string cssPath)
    // {
        // Inject the theme-dependent CSS.
        // HtmlWebView.Navigated += (sender, e) =>
        // {
        //     if (e.Result == WebNavigationResult.Success)
        //     {
        //         var js = @$"
        //             var link = document.createElement('link');
        //             link.id = '{elementId}';
        //             link.rel = 'stylesheet';
        //             link.href = '{cssPath}';
        //             document.head.appendChild(link);
        //         ";
        //         WebView.Eval(js);
        //     }
        // };
    // }

    // private string GetThemeCssPath()
    // {
    //     return "css/" + (Application.Current!.RequestedTheme == AppTheme.Dark
    //         ? "dark.css"
    //         : "light.css");
    // }

    // private void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
    // {
    //     var js = @$"
    //         var link = document.getElementById('{_THEME_CSS_LINK_ID}');
    //         link.href = '{GetThemeCssPath()}';
    //     ";
    //     // WebView.Eval(js);
    // }

    // public event PropertyChangedEventHandler PropertyChanged;

    /// <inheritdoc />
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        // Navigate to the specified route if necessary.
        // We can't directly access methods on the component, so use the service to transfer the
        // route parameter.
        if (propertyName == nameof(Route) && !string.IsNullOrEmpty(Route))
        {
            _htmlUpdaterService.Update(Route);
        }
    }
}
