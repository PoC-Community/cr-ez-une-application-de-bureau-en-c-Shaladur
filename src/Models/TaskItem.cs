using System.Data.Common;

namespace TodoListApp.Models;

public class TaskItem
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public string Id { get; set; } = string.Empty;
    public System.Collections.Generic.List<string> Tags { get; set; } = new();
}
