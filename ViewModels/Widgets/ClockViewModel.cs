using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MacroWidgets.ViewModels.Widgets;

public partial class ClockViewModel : ObservableObject
{
    private static readonly string[] ChineseDayOfWeek =
    [
        "\u661f\u671f\u65e5",
        "\u661f\u671f\u4e00",
        "\u661f\u671f\u4e8c",
        "\u661f\u671f\u4e09",
        "\u661f\u671f\u56db",
        "\u661f\u671f\u4e94",
        "\u661f\u671f\u516d"
    ];

    [ObservableProperty]
    private string _timeString = DateTime.Now.ToString("HH mm ss");

    [ObservableProperty]
    private string _hours = DateTime.Now.ToString("HH");

    [ObservableProperty]
    private string _minutes = DateTime.Now.ToString("mm");

    [ObservableProperty]
    private string _seconds = DateTime.Now.ToString("ss");

    [ObservableProperty]
    private string _dateString = FormatDateString(DateTime.Now);

    [ObservableProperty]
    private string _lunarDateString = "\u4e03\u6708\u5eff\u4e8c";

    [ObservableProperty]
    private string _dayOfWeek = ChineseDayOfWeek[(int)DateTime.Now.DayOfWeek];

    private readonly DispatcherTimer _timer;

    public ClockViewModel()
    {
        _timer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private static string FormatDateString(DateTime now)
    {
        return $"{now.Month}\u6708{now.Day}\u65e5 \u5468{GetShortDayOfWeek(now)}";
    }

    private static string GetShortDayOfWeek(DateTime now)
    {
        return now.DayOfWeek switch
        {
            System.DayOfWeek.Sunday => "\u65e5",
            System.DayOfWeek.Monday => "\u4e00",
            System.DayOfWeek.Tuesday => "\u4e8c",
            System.DayOfWeek.Wednesday => "\u4e09",
            System.DayOfWeek.Thursday => "\u56db",
            System.DayOfWeek.Friday => "\u4e94",
            System.DayOfWeek.Saturday => "\u516d",
            _ => string.Empty
        };
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var now = DateTime.Now;

        TimeString = now.ToString("HH mm ss");
        Hours = now.ToString("HH");
        Minutes = now.ToString("mm");
        Seconds = now.ToString("ss");
        DateString = FormatDateString(now);
        LunarDateString = "\u4e03\u6708\u5eff\u4e8c";
        DayOfWeek = ChineseDayOfWeek[(int)now.DayOfWeek];
    }
}
