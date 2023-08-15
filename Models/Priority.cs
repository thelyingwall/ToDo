namespace ToDo.Models
{
    public class Priority
    {
        public int PriorityId { get; set; }
        public string PriorityName { get; set; }
        public virtual ICollection<Task>? Task { get; set; }
    }
}
