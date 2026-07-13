using System;
using System.Collections.Generic;

namespace MacWidget.Models;

public class HourlyForecast
{
    public string Time { get; set; } = "";
    public string Temperature { get; set; } = "";
    public string Icon { get; set; } = "☀️"; // emoji placeholder
}

public class WeatherInfo
{
    public string City { get; set; } = "广州";
    public int CurrentTemp { get; set; } = 34;
    public int HighTemp { get; set; } = 34;
    public int LowTemp { get; set; } = 28;
    public string Condition { get; set; } = "大部晴朗";
    public string ConditionIcon { get; set; } = "☀️";
    public List<HourlyForecast> HourlyForecast { get; set; } = new();
}
