using System.Globalization;
using Galaxon.Core.Enums;
using GymCalc.Utilities;

namespace GymCalc.Converters;

public class WeightTextConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var weight = (double)value;
        var units = UnitsUtility.GetDefault().GetDescription();
        return $"{weight:F2} {units}";
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
