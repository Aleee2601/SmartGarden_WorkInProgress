using System.Collections;
using SmartGarden_WorkInProgress.Models;

namespace SmartGarden_WorkInProgress.Controls;

public partial class WeeklyStatsChart : ContentView
{
    public static readonly BindableProperty StatsProperty =
        BindableProperty.Create(nameof(Stats), typeof(IEnumerable), typeof(WeeklyStatsChart), null,
            propertyChanged: OnStatsChanged);

    public IEnumerable? Stats
    {
        get => (IEnumerable?)GetValue(StatsProperty);
        set => SetValue(StatsProperty, value);
    }

    public WeeklyStatsChart()
    {
        InitializeComponent();
    }

    private static void OnStatsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is WeeklyStatsChart chart)
        {
            chart.RenderChart();
        }
    }

    private void RenderChart()
    {
        ValuesGrid.Children.Clear();
        ValuesGrid.ColumnDefinitions.Clear();
        BarsGrid.Children.Clear();
        BarsGrid.ColumnDefinitions.Clear();
        DaysGrid.Children.Clear();
        DaysGrid.ColumnDefinitions.Clear();

        if (Stats == null) return;

        var statsList = Stats.Cast<WeeklyStat>().ToList();
        if (!statsList.Any()) return;

        var maxValue = statsList.Max(s => s.Value);

        for (int i = 0; i < statsList.Count; i++)
        {
            var stat = statsList[i];

            ValuesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            BarsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            DaysGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            // Value label
            var valueLabel = new Label
            {
                Text = stat.Value.ToString(),
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#0F172A"),
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(valueLabel, i);
            ValuesGrid.Children.Add(valueLabel);

            // Bar container with alignment at bottom
            var barContainer = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = new GridLength(Math.Max(1, (stat.Value / (double)maxValue) * 100), GridUnitType.Absolute) }
                }
            };

            var bar = new BoxView
            {
                BackgroundColor = Color.FromArgb("#1AA44B"),
                CornerRadius = 4,
                Margin = new Thickness(4, 0)
            };
            Grid.SetRow(bar, 1);
            barContainer.Children.Add(bar);

            Grid.SetColumn(barContainer, i);
            BarsGrid.Children.Add(barContainer);

            // Day label
            var dayLabel = new Label
            {
                Text = stat.Day,
                FontSize = 11,
                TextColor = Color.FromArgb("#64748B"),
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(dayLabel, i);
            DaysGrid.Children.Add(dayLabel);
        }
    }
}
