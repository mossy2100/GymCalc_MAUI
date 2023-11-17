using Galaxon.Core.Enums;
using GymCalc.Shared;

namespace GymCalc.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private string _units;

    public SettingsViewModel()
    {
        Units = UnitsUtility.GetDefault().GetDescription();
    }

    public string Units
    {
        get => _units;

        set => SetProperty(ref _units, value);
    }

    /// <inheritdoc/>
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
