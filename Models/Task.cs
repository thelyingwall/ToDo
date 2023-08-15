namespace ToDo.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; } = false;
        public DateTime CreateDate { get; set; }
        public DateTime Deadline { get; set; }
        public virtual TaskList TaskList { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
