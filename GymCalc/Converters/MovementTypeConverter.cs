using GymCalc.Enums;

namespace GymCalc.Converters;

internal class MovementTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is EMovementType mt) ? mt.ToString() : null;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        return (value is string s && Enum.TryParse(s, out EMovementType mt)) ? mt : null;
    }
}
