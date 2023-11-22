using System.Globalization;
using GymCalc.Constants;
using Microsoft.Extensions.Logging.Abstractions;

namespace GymCalc.Converters;

public class MachineTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is MachineType mt)
        {
            try
            {
                return mt.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string sValue)
        {
            try
            {
                return Enum.Parse<MachineType>(sValue);
            }
            catch (Exception)
            {
                return null;
            }
        }

        return null;
    }
}
