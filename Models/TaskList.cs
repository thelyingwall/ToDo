namespace ToDo.Models
{
    public class TaskList
    {
        public int TaskListId { get; set; }
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual ICollection<Task>? Task { get; set; }
    }
}
