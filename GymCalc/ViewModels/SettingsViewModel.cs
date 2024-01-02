using GymCalc.Enums;
using GymCalc.Services;

namespace GymCalc.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private EUnits _units = UnitsService.GetDefaultUnits();

    public EUnits Units
    {
        get => _units;

        set => SetProperty(ref _units, value);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(Units):
                Preferences.Default.Set("Units", Units.ToString());
                break;
        }
    }
}
