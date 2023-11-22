using System.Globalization;
using Galaxon.Core.Types;
using GymCalc.Constants;

namespace GymCalc.Converters;

public class BarbellTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BarbellType bt)
        {
            try
            {
                return bt.GetDescription();
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
        if (value is string s)
        {
            try
            {
                return XEnum.FindValueByDescription<BarbellType>(s);
            }
            catch (Exception)
            {
                return null;
            }
        }

        return null;
    }
}
