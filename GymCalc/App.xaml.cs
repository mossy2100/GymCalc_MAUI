namespace GymCalc;

public partial class App : Application
{
    internal const double Spacing = 10;
    internal const double DoubleSpacing = 2 * Spacing;
    internal static Thickness Padding = new (Spacing);

    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    internal static int GetNumColumns()
    {
        return DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Portrait
            ? 1
            : 2;
    }
}
