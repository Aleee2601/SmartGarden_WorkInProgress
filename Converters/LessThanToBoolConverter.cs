using System.Globalization;

namespace SmartGarden_WorkInProgress.Converters;

public class LessThanToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue && parameter is string threshold && int.TryParse(threshold, out int thresholdValue))
        {
            return intValue < thresholdValue;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
