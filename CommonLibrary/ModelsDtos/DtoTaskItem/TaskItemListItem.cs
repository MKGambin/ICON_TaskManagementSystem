using CommonLibrary.Models;
using CommonLibrary.ModelsDtos.DtoUser;

namespace CommonLibrary.ModelsDtos.DtoTaskItem
{
    public class TaskItemListItem
    {
        public TaskItemListItem() { }
        public TaskItemListItem(TaskItem taskItem)
        {
            User = taskItem.User is not null ? new(taskItem.User) : null;
            Identifier = taskItem.Identifier;
            Name = taskItem.Name;
            Description = taskItem.Description;
            TaskItemStatus = taskItem.TaskItemStatus;
        }

        public UserListItem? User { get; set; }
        public string? Identifier { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TaskItemStatusType? TaskItemStatus { get; set; }
        public string? TaskItemStatusText =>
            TaskItemStatus is not null ? $"{(int)TaskItemStatus} = {TaskItemStatus}"
            : $"N/a";
    }
}
