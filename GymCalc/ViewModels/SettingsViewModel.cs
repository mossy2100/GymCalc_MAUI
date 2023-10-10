using Galaxon.Core.Enums;
using GymCalc.Utilities;

namespace GymCalc.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private string _units;

    public string Units
    {
        get => _units;

        set => SetProperty(ref _units, value);
    }

    public SettingsViewModel()
    {
        Units = UnitsUtility.GetDefault().GetDescription();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(Units):
                Preferences.Default.Set("Units", Units);
                break;
        }
    }
}
