using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using TodoListApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System;
using System.Threading.Tasks;

namespace TodoListApp;

public partial class MainWindow : Window
{
    private ObservableCollection<TaskItem> _tasks = new();
    private System.Collections.Generic.List<TaskItem> _allTasks = new();

    public MainWindow()
    {
        InitializeComponent();
        TaskList.ItemsSource = _tasks;

    AddButton.Click += OnAddClick;
    DeleteButton.Click += OnDeleteClick;
    SaveButton.Click += OnSaveClick;
    FilterButton.Click += OnFilterClick;
    ClearFilterButton.Click += OnClearFilterClick;

    LoadTasks();
    }

    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        var title = TaskInput.Text?.Trim();
        var tagsText = string.Empty;
        try { tagsText = TagInput.Text ?? string.Empty; } catch { }

        if (!string.IsNullOrWhiteSpace(title))
        {
            var tags = ParseTags(tagsText);
            var item = new TaskItem { Title = title, Tags = tags };
            _allTasks.Add(item);
            _tasks.Add(item);
            TaskInput.Text = string.Empty;
            try { TagInput.Text = string.Empty; } catch { }
        }
    }

        private System.Collections.Generic.List<string> ParseTags(string input)
    {
        var result = new System.Collections.Generic.List<string>();
        if (string.IsNullOrWhiteSpace(input)) return result;

        var parts = input.Split(',');
        foreach (var p in parts)
        {
            var t = p.Trim();
            if (t.Length == 0) continue;
            if (!result.Contains(t))
                result.Add(t);
        }
        return result;
    }

    private void LoadTasks()
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "data");
        var path = Path.Combine(dir, "tasks.json");

        if (!File.Exists(path))
        {
            try { StatusText.Text = "No saved tasks found."; } catch { }
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<List<TaskItem>>(json, options);

            _tasks.Clear();
            _allTasks.Clear();
            if (list != null)
            {
                foreach (var item in list)
                {
                    _allTasks.Add(item);
                    _tasks.Add(item);
                }

                try { StatusText.Text = $"Loaded {_tasks.Count} tasks from: {path}"; } catch { }
            }
            else
            {
                try { StatusText.Text = "No tasks found in file."; } catch { }
            }
        }
        catch (System.Text.Json.JsonException jex)
        {
            Console.Error.WriteLine($"Failed to deserialize tasks.json: {jex}");
            try { StatusText.Text = "Invalid tasks.json — started with empty list."; } catch { }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to load tasks: {ex}");
            try { StatusText.Text = $"Failed to load tasks: {ex.Message}"; } catch { }
        }
    }

    private void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (TaskList.SelectedItem is TaskItem selected)
        {
            _tasks.Remove(selected);
            try { _allTasks.Remove(selected); } catch { }
        }
    }

    private void OnFilterClick(object? sender, RoutedEventArgs e)
    {
        var tag = string.Empty;
        try { tag = FilterInput.Text?.Trim() ?? string.Empty; } catch { }
        ApplyFilter(tag);
    }

    private void OnClearFilterClick(object? sender, RoutedEventArgs e)
    {
        try { FilterInput.Text = string.Empty; } catch { }
        ApplyFilter(string.Empty);
    }

    private void ApplyFilter(string tag)
    {
        _tasks.Clear();
        if (string.IsNullOrWhiteSpace(tag))
        {
            foreach (var t in _allTasks) _tasks.Add(t);
            try { StatusText.Text = "Showing all tasks."; } catch { }
            return;
        }

        var lowered = tag.Trim();
        foreach (var t in _allTasks)
        {
            if (t.Tags != null && t.Tags.Exists(x => string.Equals(x, lowered, StringComparison.OrdinalIgnoreCase)))
                _tasks.Add(t);
        }

        try { StatusText.Text = $"Filtered by tag: {tag} (showing {_tasks.Count})"; } catch { }
    }

        private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "data");

        try
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, "tasks.json");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var list = new List<TaskItem>(_allTasks);

            var json = JsonSerializer.Serialize(list, options);

            await File.WriteAllTextAsync(path, json);

            Console.WriteLine($"Tasks saved to: {path}");

            try
            {
                SaveButton.Content = "Saved";
                await Task.Delay(1200);
                SaveButton.Content = "Save to JSON";
            }
            catch
            {
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to save tasks: {ex}");

            try
            {
                SaveButton.Content = "Error";
                await Task.Delay(1500);
                SaveButton.Content = "Save to JSON";
            }
            catch
            {
            }
        }
    }

}
