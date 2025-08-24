using System.Text.Json.Serialization;

namespace CommonLibrary.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public ICollection<TaskItem>? TaskItems { get; set; } = [];
    }
}
