using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MacWidget.Models;
using MacWidget.Services;

namespace MacWidget.ViewModels.Widgets;

public partial class RemindersViewModel : ObservableObject
{
    [ObservableProperty] private int _pendingCount;

    [ObservableProperty] private string _headerText = "提醒事项";

    [ObservableProperty] private string _newItemText = "";

    public ObservableCollection<ReminderItem> Items { get; } = [];

    public RemindersViewModel()
    {
        var today = DateTime.Now;

        Items.Add(new ReminderItem
        {
            Title = "Widget 测试",
            IsCompleted = false,
            DueDate = today
        });

        Items.Add(new ReminderItem
        {
            Title = "Design",
            IsCompleted = false,
            DueDate = today.AddDays(1)
        });

        Items.Add(new ReminderItem
        {
            Title = "Monitor iPhone",
            IsCompleted = false,
            DueDate = today.AddDays(2)
        });

        RecalculatePendingCount();
    }

    [RelayCommand]
    private void AddItem()
    {
        if (string.IsNullOrWhiteSpace(NewItemText)) return;

        Items.Add(new ReminderItem
        {
            Title = NewItemText.Trim(),
            IsCompleted = false,
            DueDate = DateTime.Now
        });

        NewItemText = "";
        RecalculatePendingCount();
        SaveItems();
    }

    [RelayCommand]
    private void RemoveItem(ReminderItem? item)
    {
        if (item is null) return;

        Items.Remove(item);
        RecalculatePendingCount();
        SaveItems();
    }

    [RelayCommand]
    private void ToggleComplete(ReminderItem? item)
    {
        if (item is null) return;

        item.IsCompleted = !item.IsCompleted;
        RecalculatePendingCount();
        SaveItems();
    }

    private void RecalculatePendingCount()
    {
        PendingCount = Items.Count(i => !i.IsCompleted);
    }

    private void SaveItems()
    {
        // TODO: 持久化到文件（后续在 Persistence 服务中扩展）
    }
}
