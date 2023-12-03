using System.Globalization;
using GymCalc.Constants;

namespace GymCalc.Converters;

public class BarbellTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is BarbellType bt)
        {
            return bt.ToString();
        }

        return null;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        if (value is string s)
        {
            return Enum.TryParse(s, out BarbellType bt) ? bt : null;
        }

        return null;
    }
}
