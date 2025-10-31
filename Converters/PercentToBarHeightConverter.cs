using System.Globalization;

namespace SmartGarden_WorkInProgress.Converters;

public class PercentToBarHeightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            // Normalize to 0-100 range for GridLength
            var normalized = Math.Max(0, Math.Min(100, intValue));
            return new GridLength(normalized, GridUnitType.Star);
        }
        return new GridLength(1, GridUnitType.Star);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
