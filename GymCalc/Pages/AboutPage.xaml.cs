using GymCalc.Utilities;
using HtmlAgilityPack;

namespace GymCalc.Pages;

public partial class AboutPage : ContentPage
{
    private bool _textLoaded;

    public AboutPage()
    {
        InitializeComponent();

        Application.Current!.RequestedThemeChanged += OnThemeChange;
    }

    private void OnThemeChange(object sender, AppThemeChangedEventArgs e)
    {
        TextUtility.SetLayoutTextColor(AboutLayout);
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        if (!_textLoaded)
        {
            await TextUtility.LoadHtmlIntoLayout("About.html", AboutLayout);
            _textLoaded = true;
        }
    }
}
