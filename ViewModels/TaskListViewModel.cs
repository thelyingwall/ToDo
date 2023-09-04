using Microsoft.AspNetCore.Mvc.Rendering;

namespace ToDo.ViewModels
{
    public class TaskListViewModel
    {
        public ToDo.Models.TaskList? TaskList { get; set; }
        public PaginationViewModel<Models.Task>? Tasks { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public int? CategoryId { get; set; }
        public List<SelectListItem>? Priorities { get; set; }
        public int? PriorityId { get; set; }
        public int? progress { get; set; }
        public string? searchString { get; set;}
        public string? ajdi { get; set;}

    }
}
