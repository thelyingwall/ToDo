using System.ComponentModel.DataAnnotations;

namespace ToDo.Models
{
    public class TaskList
    {
        public int TaskListId { get; set; }
        [MaxLength(30, ErrorMessage = "Maksymalna ilość znaków wynosi {1}")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual ICollection<Task>? Tasks { get; set; }
    }
}
