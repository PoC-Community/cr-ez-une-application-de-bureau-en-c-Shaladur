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

    public MainWindow()
    {
        InitializeComponent();
        TaskList.ItemsSource = _tasks;

        AddButton.Click += OnAddClick;
        DeleteButton.Click += OnDeleteClick;
        SaveButton.Click += OnSaveClick;
        LoadTasks();
    }

    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TaskInput.Text))
        {
            _tasks.Add(new TaskItem { Title = TaskInput.Text });
            TaskInput.Text = string.Empty;
        }
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
            if (list != null)
            {
                foreach (var item in list)
                    _tasks.Add(item);

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
        }
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

            var list = new List<TaskItem>(_tasks);

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
