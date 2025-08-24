using CommonLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.ModelsDtos.DtoTaskItem
{
    public class TaskItemFormItem
    {
        public TaskItemFormItem() { }
        public TaskItemFormItem(TaskItem taskItem)
        {
            User = taskItem.User!.Identifier;
            Identifier = taskItem.Identifier;
            Name = taskItem.Name;
            Description = taskItem.Description;
            TaskItemStatus = taskItem.TaskItemStatus;
        }

        [Required]
        public string? User { get; set; }

        public string? Identifier { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public TaskItemStatusType? TaskItemStatus { get; set; }
    }
}
