using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MacroWidgets.ViewModels.Widgets;

public partial class CalendarViewModel : ObservableObject
{
    private static readonly string[] ChineseMonths =
    [
        "一月", "二月", "三月", "四月", "五月", "六月",
        "七月", "八月", "九月", "十月", "十一月", "十二月"
    ];

    [ObservableProperty] private int _currentYear;

    [ObservableProperty] private int _currentMonth;

    [ObservableProperty] private int _today;

    [ObservableProperty] private string _monthName = "";

    [ObservableProperty] private List<CalendarDay> _days = [];

    public CalendarViewModel()
    {
        var now = DateTime.Now;
        _currentYear = now.Year;
        _currentMonth = now.Month;
        _today = now.Day;
        _monthName = ChineseMonths[_currentMonth - 1];
        _days = GenerateDays(_currentYear, _currentMonth);
    }

    private static List<CalendarDay> GenerateDays(int year, int month)
    {
        var days = new List<CalendarDay>(42);
        var firstDayOfMonth = new DateTime(year, month, 1);

        // Monday = 0, Sunday = 6
        int startOffset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

        int daysInMonth = DateTime.DaysInMonth(year, month);
        int daysInPrevMonth = DateTime.DaysInMonth(
            month == 1 ? year - 1 : year,
            month == 1 ? 12 : month - 1);

        // Leading days from previous month
        for (int i = startOffset - 1; i >= 0; i--)
        {
            days.Add(new CalendarDay(daysInPrevMonth - i, false, false));
        }

        // Current month days
        for (int d = 1; d <= daysInMonth; d++)
        {
            bool isToday = (d == DateTime.Now.Day
                           && month == DateTime.Now.Month
                           && year == DateTime.Now.Year);
            days.Add(new CalendarDay(d, true, isToday));
        }

        // Trailing days to fill 42 cells
        int remaining = 42 - days.Count;
        for (int i = 1; i <= remaining; i++)
        {
            days.Add(new CalendarDay(i, false, false));
        }

        return days;
    }
}

public record CalendarDay(int Day, bool IsCurrentMonth, bool IsToday);
