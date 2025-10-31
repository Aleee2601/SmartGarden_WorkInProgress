using System.Globalization;
using SmartGarden_WorkInProgress.ViewModels;

namespace SmartGarden_WorkInProgress.Views;

public partial class PlantDetailPage : ContentPage
{
    public PlantDetailPage(PlantDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Add converters to page resources
        Resources.Add("TimeSpanToMinutesConverter", new TimeSpanToMinutesConverter());
    }
}

public class TimeSpanToMinutesConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            return (int)timeSpan.TotalMinutes;
        }
        return 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
