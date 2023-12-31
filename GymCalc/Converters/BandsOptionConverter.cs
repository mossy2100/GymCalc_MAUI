using GymCalc.Enums;

namespace GymCalc.Converters;

internal class BandsOptionConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is EBandsOption bo) ? bo.ToString() : null;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        return (value is string s && Enum.TryParse(s, out EBandsOption bo)) ? bo : null;
    }
}
