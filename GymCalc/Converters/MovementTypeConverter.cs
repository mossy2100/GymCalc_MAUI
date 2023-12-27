using System.Globalization;
using GymCalc.Enums;

namespace GymCalc.Converters;

public class MovementTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EMovementType mt)
        {
            return mt.ToString();
        }

        return null;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        if (value is string s)
        {
            return Enum.TryParse(s, out EMovementType mt) ? mt : null;
        }

        return null;
    }
}
