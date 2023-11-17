using System.Globalization;
using GymCalc.Constants;

namespace GymCalc.Converters;

public class MachineTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is MachineType mt)
        {
            return mt.ToString();
        }
        return null;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string sValue)
        {
            return Enum.Parse<MachineType>(sValue);
        }
        return null;
    }
}
