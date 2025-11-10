using System.ComponentModel;

namespace TodoListApp.Models;

public class TaskItem : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private bool _isCompleted = false;
    private string _id = string.Empty;
    private System.Collections.Generic.List<string> _tags = new();
    private System.DateTime? _dueDate;
    private bool _isEditing = false;

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(IsOverdue));
            }
        }
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (_isCompleted != value)
            {
                _isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
                OnPropertyChanged(nameof(IsOverdue));
            }
        }
    }

    public string Id
    {
        get => _id;
        set
        {
            if (_id != value)
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
    }

    public System.Collections.Generic.List<string> Tags
    {
        get => _tags;
        set
        {
            if (_tags != value)
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }
    }

    public System.DateTime? DueDate
    {
        get => _dueDate;
        set
        {
            if (_dueDate != value)
            {
                _dueDate = value;
                OnPropertyChanged(nameof(DueDate));
                OnPropertyChanged(nameof(IsOverdue));
            }
        }
    }

    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsOverdue => DueDate.HasValue && DueDate.Value.Date < System.DateTime.Today && !IsCompleted;

    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsEditing
    {
        get => _isEditing;
        set
        {
            if (_isEditing != value)
            {
                _isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
