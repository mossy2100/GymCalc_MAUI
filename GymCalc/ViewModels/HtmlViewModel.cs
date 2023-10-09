namespace GymCalc.ViewModels;

public class HtmlViewModel : BaseViewModel
{
    private string _title;

    public string Title
    {
        get => _title;

        set => SetProperty(ref _title, value);
    }
}
