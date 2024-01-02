using GymCalc.Enums;

namespace GymCalc.Converters;

internal class BarbellTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is EBarbellType bt) ? bt.ToString() : null;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        return (value is string s && Enum.TryParse(s, out EBarbellType bt)) ? bt : null;
    }
}
