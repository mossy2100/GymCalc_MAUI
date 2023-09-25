namespace GymCalc.Pages;

[QueryProperty(nameof(Title), "title")]
[QueryProperty(nameof(FileName), "fileName")]
public partial class HtmlPage : ContentPage
{
    private const string _STYLES_CSS_LINK_ID = "styles-css";

    private const string _THEME_CSS_LINK_ID = "theme-css";

    private string _fileName;

    public string FileName
    {
        get => _fileName;

        set
        {
            value = $"html/{value}";
            if (_fileName != value)
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }
    }

    public HtmlPage()
    {
        InitializeComponent();
        BindingContext = this;

        // Inject CSS files.
        InjectCss(_STYLES_CSS_LINK_ID, "css/styles.css");
        InjectCss(_THEME_CSS_LINK_ID, GetThemeCssPath());

        // React to theme change.
        Application.Current!.RequestedThemeChanged += OnRequestedThemeChanged;
    }

    private void InjectCss(string elementId, string cssPath)
    {
        // Inject the theme-dependent CSS.
        WebView.Navigated += (sender, e) =>
        {
            if (e.Result == WebNavigationResult.Success)
            {
                var js = @$"
                    var link = document.createElement('link');
                    link.id = '{elementId}';
                    link.rel = 'stylesheet';
                    link.href = '{cssPath}';
                    document.head.appendChild(link);
                ";
                WebView.Eval(js);
            }
        };
    }

    private string GetThemeCssPath()
    {
        return "css/" + (Application.Current!.RequestedTheme == AppTheme.Dark
            ? "dark.css"
            : "light.css");
    }

    private void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
    {
        var js = @$"
            var link = document.getElementById('{_THEME_CSS_LINK_ID}');
            link.href = '{GetThemeCssPath()}';
        ";
        WebView.Eval(js);
    }
}
