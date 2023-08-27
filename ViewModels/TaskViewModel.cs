using Microsoft.AspNetCore.Mvc.Rendering;
using ToDo.Models;

namespace ToDo.ViewModels
{
    public class TaskViewModel
    {
        public ToDo.Models.Task? Task { get; set; }
        public List<SelectListItem>? Priorities { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public int? PriorityId { get; set; }
        public int? CategoryId { get; set; }
        public int? TaskListId { get; set; }

    }
}
