namespace GymCalc;

public partial class App : Application
{
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
