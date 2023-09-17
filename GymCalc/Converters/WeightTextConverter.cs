using System.Globalization;
using GymCalc.Constants;

namespace GymCalc.Converters;

public class WeightTextConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var weight = (double)value;
        var units = Units.GetPreferred();
        return $"{weight:F2} {units}";
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
