using Microsoft.AspNetCore.Mvc.Rendering;

namespace ToDo.ViewModels
{
    public class TaskListViewModel
    {
        public ToDo.Models.TaskList? TaskList { get; set; }
        public List<Models.Task>? Tasks { get; set; }
    }
}
