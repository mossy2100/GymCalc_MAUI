using GymCalc.Utilities;
using HtmlAgilityPack;

namespace GymCalc.Pages;

public partial class AboutPage : ContentPage
{
    private bool _textLoaded;

    public AboutPage()
    {
        InitializeComponent();

        var app = Application.Current;
        if (app != null)
        {
            app.RequestedThemeChanged += OnThemeChange;
        }
    }

    private void OnThemeChange(object sender, AppThemeChangedEventArgs e)
    {
        SetTextColor();
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        if (!_textLoaded)
        {
            await LoadAboutText();
            _textLoaded = true;
        }
    }

    private async Task LoadAboutText()
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync("About.html");

        var doc = new HtmlDocument();
        doc.Load(stream);

        // Recursively process HTML document.
        var spans = new List<List<Span>>();
        TextUtility.ProcessHtmlDocument(doc.DocumentNode, 14, FontAttributes.None, ref spans);

        // Construct the labels and add them to the document.
        foreach (var spanList in spans)
        {
            var label = new Label();
            var fstr = new FormattedString();
            foreach (var span in spanList)
            {
                fstr.Spans.Add(span);
                Console.WriteLine($"span text = {span.Text}");
            }
            label.FormattedText = fstr;
            AboutLayout.Add(label);
        }

        SetTextColor();
    }

    private void SetTextColor()
    {
        // Get the text color.
        var textColor = Application.Current!.RequestedTheme == AppTheme.Light
            ? Colors.Black
            : Colors.White;

        // Change the color of every span.
        foreach (var item in AboutLayout)
        {
            if (item is not Label label)
            {
                continue;
            }

            foreach (var span in label.FormattedText.Spans)
            {
                span.TextColor = textColor;
            }
        }
    }
}
