using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.CompilerServices;
using ToDo.Models;
using ToDo.Models.Validation;

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
        [DeadlineAfterCreateDateAttribute(ErrorMessage = "Wybierz przyszłą datę.")]
        public DateTime Deadline { get; set; }= DateTime.Today.AddDays(1);

        public override string? ToString()
        {
            return $"=============================================================================================\n" +
                $"{Task.Title} {Task.Description} {Task.IsDone} {Task.CreateDate} {Task.Deadline} {Task.TaskList.Title} {Task.Priority.PriorityName} {Task.Category.CategoryName}\n" +
                $"=============================================================================================";
        }
    }
}
