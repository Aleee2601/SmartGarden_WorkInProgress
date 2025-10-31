using System.Globalization;
using SmartGarden_WorkInProgress.ViewModels;

namespace SmartGarden_WorkInProgress.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Add converters to page resources
        Resources.Add("RoomFilterBackgroundConverter", new RoomFilterBackgroundConverter());
        Resources.Add("RoomFilterTextConverter", new RoomFilterTextConverter());
        Resources.Add("CountToVisibilityConverter", new CountToVisibilityConverter());
        Resources.Add("IndoorOutdoorConverter", new IndoorOutdoorConverter());
    }
}

public class RoomFilterBackgroundConverter : IMultiValueConverter
{
    public object? Convert(object?[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Length == 0) return Color.FromArgb("#F3F4F6");
        var isSelected = values[0] as bool? ?? false;
        return isSelected ? Color.FromArgb("#1AA44B") : Color.FromArgb("#F3F4F6");
    }

    public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class RoomFilterTextConverter : IMultiValueConverter
{
    public object? Convert(object?[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Length == 0) return Color.FromArgb("#64748B");
        var isSelected = values[0] as bool? ?? false;
        return isSelected ? Colors.White : Color.FromArgb("#64748B");
    }

    public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class CountToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            return count > 0;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IndoorOutdoorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isIndoor)
        {
            return isIndoor ? "INDOOR" : "OUTDOOR";
        }
        return "INDOOR";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
