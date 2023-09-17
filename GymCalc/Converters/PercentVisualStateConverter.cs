using System.Globalization;

namespace GymCalc.Converters;

public class PercentVisualStateConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var selectedPercent = (int)value;
        if (int.TryParse(parameter as string, out var targetPercent))
        {
            return selectedPercent == targetPercent
                ? VisualStateManager.CommonStates.Selected
                : VisualStateManager.CommonStates.Normal;
        }

        // Default.
        return VisualStateManager.CommonStates.Normal;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
