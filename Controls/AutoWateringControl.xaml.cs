using System.Globalization;

namespace SmartGarden_WorkInProgress.Controls;

public partial class AutoWateringControl : ContentView
{
    public static readonly BindableProperty IsOnProperty =
        BindableProperty.Create(nameof(IsOn), typeof(bool), typeof(AutoWateringControl), false);

    public static readonly BindableProperty IntensityProperty =
        BindableProperty.Create(nameof(Intensity), typeof(double), typeof(AutoWateringControl), 50.0);

    public bool IsOn
    {
        get => (bool)GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }

    public double Intensity
    {
        get => (double)GetValue(IntensityProperty);
        set => SetValue(IntensityProperty, value);
    }

    public AutoWateringControl()
    {
        InitializeComponent();
        Resources.Add("SegmentBackgroundConverter", new SegmentBackgroundConverter());
        Resources.Add("SegmentTextColorConverter", new SegmentTextColorConverter());
    }
}

public class SegmentBackgroundConverter : IMultiValueConverter
{
    public object? Convert(object?[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Length != 2) return Colors.Transparent;

        var isOn = values[0] as bool? ?? false;
        var isOnSegment = values[1] as bool? ?? false;

        return (isOn == isOnSegment)
            ? Color.FromArgb("#1AA44B")
            : Colors.Transparent;
    }

    public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SegmentTextColorConverter : IMultiValueConverter
{
    public object? Convert(object?[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Length != 2) return Color.FromArgb("#64748B");

        var isOn = values[0] as bool? ?? false;
        var isOnSegment = values[1] as bool? ?? false;

        return (isOn == isOnSegment)
            ? Colors.White
            : Color.FromArgb("#64748B");
    }

    public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
