using System.Globalization;
using Galaxon.Core.Types;
using GymCalc.Shared;

namespace GymCalc.Converters;

public class WeightTextConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal weight)
        {
            string units = UnitsUtility.GetDefault().GetDescription();
            return $"{weight:F2} {units}";
        }

        return null;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        return null;
    }
}
