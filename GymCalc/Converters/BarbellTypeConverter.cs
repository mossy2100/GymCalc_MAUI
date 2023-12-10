using System.Globalization;
using GymCalc.Enums;

namespace GymCalc.Converters;

public class BarbellTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EBarbellType bt)
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
            return Enum.TryParse(s, out EBarbellType bt) ? bt : null;
        }

        return null;
    }
}
