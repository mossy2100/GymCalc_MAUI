namespace GymCalc.Services;

public class HtmlUpdaterService
{
    public event Action OnUpdate;

    public string Route { get; set; }

    public void Update(string route)
    {
        Route = route;
        OnUpdate?.Invoke();
    }
}
