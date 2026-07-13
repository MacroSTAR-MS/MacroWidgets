using System;

namespace MacroWidgets.Models;

public class ReminderItem
{
    public string Title { get; set; } = "";
    public bool IsCompleted { get; set; }
    public DateTime DueDate { get; set; }
}
