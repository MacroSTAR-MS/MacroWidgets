using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using MacroWidgets.Models;

namespace MacroWidgets.ViewModels.Widgets;

public partial class WeatherViewModel : ObservableObject
{
    [ObservableProperty]
    private WeatherInfo _weatherInfo;

    public WeatherViewModel()
    {
        WeatherInfo = new WeatherInfo
        {
            City = "广州",
            CurrentTemp = 34,
            HighTemp = 34,
            LowTemp = 28,
            Condition = "大部晴朗",
            ConditionIcon = "☀️",
            HourlyForecast = new List<HourlyForecast>
            {
                new() { Time = "14时", Temperature = "34°", Icon = "☀️" },
                new() { Time = "15时", Temperature = "33°", Icon = "⛅" },
                new() { Time = "16时", Temperature = "32°", Icon = "⛅" },
                new() { Time = "17时", Temperature = "31°", Icon = "🌤" },
                new() { Time = "18时", Temperature = "29°", Icon = "🌙" },
                new() { Time = "19时", Temperature = "28°", Icon = "🌙" },
            }
        };
    }
}
