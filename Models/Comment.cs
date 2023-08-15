namespace ToDo.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public virtual Task Task { get; set; }
    }
}
