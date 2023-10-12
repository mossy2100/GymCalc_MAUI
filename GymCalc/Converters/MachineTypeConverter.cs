using System.Globalization;
using GymCalc.Constants;

namespace GymCalc.Converters;

public class MachineTypeConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((MachineType)value).ToString();
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Enum.Parse<MachineType>((string)value);
    }
}
