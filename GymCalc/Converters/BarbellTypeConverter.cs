using GymCalc.Enums;

namespace GymCalc.Converters;

internal class BarbellTypeConverter : IValueConverter
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
        if (value is string s && Enum.TryParse(s, out EBarbellType bt))
        {
            return bt;
        }

        return null;
    }
}
