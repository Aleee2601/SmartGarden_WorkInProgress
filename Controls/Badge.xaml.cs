namespace SmartGarden_WorkInProgress.Controls;

public partial class Badge : Frame
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(Badge), string.Empty);

    public static readonly BindableProperty VariantProperty =
        BindableProperty.Create(nameof(Variant), typeof(BadgeVariant), typeof(Badge), BadgeVariant.Success,
            propertyChanged: OnVariantChanged);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public BadgeVariant Variant
    {
        get => (BadgeVariant)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public Color TextColor { get; set; } = Colors.White;

    public Badge()
    {
        InitializeComponent();
        UpdateColors();
    }

    private static void OnVariantChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is Badge badge)
        {
            badge.UpdateColors();
        }
    }

    private void UpdateColors()
    {
        switch (Variant)
        {
            case BadgeVariant.Success:
                BackgroundColor = Color.FromArgb("#1AA44B");
                TextColor = Colors.White;
                break;
            case BadgeVariant.Neutral:
                BackgroundColor = Color.FromArgb("#E5E7EB");
                TextColor = Color.FromArgb("#6B7280");
                break;
            case BadgeVariant.Danger:
                BackgroundColor = Color.FromArgb("#E11D48");
                TextColor = Colors.White;
                break;
        }
    }
}

public enum BadgeVariant
{
    Success,
    Neutral,
    Danger
}
