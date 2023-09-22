namespace GymCalc.Pages;

[QueryProperty(nameof(Title), "title")]
[QueryProperty(nameof(FileName), "fileName")]
public partial class HtmlPage : ContentPage
{
    private string _title;

    public string Title
    {
        get => _title;

        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    private string _fileName;

    public string FileName
    {
        get => _fileName;

        set
        {
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
    }
}
