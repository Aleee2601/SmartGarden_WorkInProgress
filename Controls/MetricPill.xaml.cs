namespace SmartGarden_WorkInProgress.Controls;

public partial class MetricPill : Frame
{
    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(MetricPill), string.Empty);

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(MetricPill), string.Empty);

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(MetricPill), string.Empty);

    public static readonly BindableProperty UnitProperty =
        BindableProperty.Create(nameof(Unit), typeof(string), typeof(MetricPill), string.Empty);

    public static readonly BindableProperty CaptionProperty =
        BindableProperty.Create(nameof(Caption), typeof(string), typeof(MetricPill), string.Empty);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public MetricPill()
    {
        InitializeComponent();
    }
}
