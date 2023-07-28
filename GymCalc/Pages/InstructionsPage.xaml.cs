using GymCalc.Utilities;

namespace GymCalc.Pages;

public partial class InstructionsPage : ContentPage
{
    private bool _textLoaded;

    public InstructionsPage()
    {
        InitializeComponent();

        Application.Current!.RequestedThemeChanged += OnThemeChange;
    }

    private void OnThemeChange(object sender, AppThemeChangedEventArgs e)
    {
        TextUtility.SetLayoutTextColor(InstructionsLayout);
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        if (!_textLoaded)
        {
            await TextUtility.LoadHtmlIntoLayout("Instructions.html", InstructionsLayout);
            _textLoaded = true;
        }
    }
}
