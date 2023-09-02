namespace ToDo.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsDone { get; set; } = false;
        public DateTime CreateDate { get; set; }
        public DateTime Deadline { get; set; }
        public int TaskListId { get; set; }
        public virtual TaskList? TaskList { get; set; }
        public int PriorityId { get; set; }
        public virtual Priority? Priority { get; set; }

        public override string? ToString()
        {
            return $"=============================================================================================\n" +
                $"{Title} {Description} {IsDone} {CreateDate} {Deadline} {TaskList.Title} {Priority.PriorityName}\n" +
                $"=============================================================================================";
        }
    }
}
