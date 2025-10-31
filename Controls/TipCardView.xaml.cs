namespace SmartGarden_WorkInProgress.Controls;

public partial class TipCardView : Frame
{
    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(TipCardView), string.Empty);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TipCardView), string.Empty);

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(TipCardView), string.Empty);

    public static readonly BindableProperty EmphasisProperty =
        BindableProperty.Create(nameof(Emphasis), typeof(bool), typeof(TipCardView), false,
            propertyChanged: OnEmphasisChanged);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool Emphasis
    {
        get => (bool)GetValue(EmphasisProperty);
        set => SetValue(EmphasisProperty, value);
    }

    public TipCardView()
    {
        InitializeComponent();
        UpdateBackground();
    }

    private static void OnEmphasisChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TipCardView card)
        {
            card.UpdateBackground();
        }
    }

    private void UpdateBackground()
    {
        if (Emphasis)
        {
            Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop { Color = Color.FromArgb("#0E7A2B"), Offset = 0.0f },
                    new GradientStop { Color = Color.FromArgb("#2DB34A"), Offset = 1.0f }
                }
            };
        }
        else
        {
            BackgroundColor = Color.FromArgb("#F3F4F6");
            var labels = this.GetVisualTreeDescendants().OfType<Label>();
            foreach (var label in labels)
            {
                label.TextColor = Color.FromArgb("#0F172A");
            }
        }
    }
}
