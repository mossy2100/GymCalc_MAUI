namespace GymCalc.Services;

public class HtmlUpdaterService
{
    public string? Route { get; set; }

    public AppTheme Theme { get; set; }

    public event Action? OnUpdateRoute;

    public event Action? OnUpdateTheme;

    public void UpdateRoute(string? route)
    {
        Route = route;
        OnUpdateRoute?.Invoke();
    }

    public void UpdateTheme(AppTheme theme)
    {
        Theme = theme;
        OnUpdateTheme?.Invoke();
    }
}
