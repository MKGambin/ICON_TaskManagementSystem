using System.Text.Json.Serialization;

namespace CommonLibrary.Models
{
    public enum TaskItemStatusType
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4,
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TaskItemStatusType TaskItemStatus { get; set; }
    }
}
