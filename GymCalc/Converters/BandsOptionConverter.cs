using GymCalc.Enums;

namespace GymCalc.Converters;

internal class BandsOptionConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EBandsOption bo)
        {
            return bo.ToString();
        }

        return null;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        if (value is string s && Enum.TryParse(s, out EBandsOption bo))
        {
            return bo;
        }

        return null;
    }
}
