using GymCalc.Enums;

namespace GymCalc.Converters;

internal class MovementTypeConverter : IValueConverter
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
        if (value is string s && Enum.TryParse(s, out EMovementType mt))
        {
            return mt;
        }

        return null;
    }
}
