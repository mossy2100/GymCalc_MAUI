using System.Reflection;

namespace GymCalc.Pages;

public partial class InstructionsPage : ContentPage
{
    public InstructionsPage()
    {
        InitializeComponent();
        // LoadHtmlFile();
    }

    private void LoadHtmlFile()
    {
        Assembly assembly = typeof(InstructionsPage).GetTypeInfo().Assembly;

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            System.Diagnostics.Debug.WriteLine(resourceName);
        }

        Stream? stream = assembly.GetManifestResourceStream("Instructions.html");
        if (stream == null)
        {
            return;
        }

        using (var reader = new StreamReader(stream))
        {
            string html = reader.ReadToEnd();
            WebView.Source = new HtmlWebViewSource { Html = html };
        }
    }
}
