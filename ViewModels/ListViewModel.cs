using Microsoft.AspNetCore.Mvc.Rendering;

namespace ToDo.ViewModels
{
    public class ListViewModel
    {
        public List<Models.TaskList>? TaskLists { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public int? CategoryId { get; set; }
    }
}
