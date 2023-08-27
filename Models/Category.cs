namespace ToDo.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<Task>? Tasks { get; set; }
    }
}
