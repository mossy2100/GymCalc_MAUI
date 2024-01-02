using GymCalc.Enums;

namespace GymCalc.Converters;

internal class UnitsConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is EUnits units) ? units.ToString() : null;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        return (value is string str && Enum.TryParse(str, out EUnits units)) ? units : null;
    }
}
