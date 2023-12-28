using GymCalc.Services;

namespace GymCalc.Converters;

public class WeightTextConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal weight)
        {
            string sUnits = UnitsService.GetDefaultUnitsSymbol();
            return $"{weight:F2} {sUnits}";
        }

        return null;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        return null;
    }
}
