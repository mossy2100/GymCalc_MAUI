namespace GymCalc.Converters;

internal class BooleanConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is bool b) ? (b ? "True" : "False") : null;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        return value is "True";
    }
}
