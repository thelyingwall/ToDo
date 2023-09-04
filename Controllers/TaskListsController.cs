using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.Models;
using ToDo.ViewModels;

namespace ToDo.Controllers
{
    //[Authorize]
    public class TaskListsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TaskLists
        public async Task<IActionResult> Index(string? sortOrder, string? searchString)
        {
            if (_context.TaskList != null)
            {
                ListViewModel listViewModel = new ListViewModel();
                ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "Title_desc" : "";
                ViewBag.CreateDateSortParm = sortOrder == "CreateDate" ? "CreateDate_desc" : "CreateDate";
                ViewBag.CategorySortParm = sortOrder == "Category" ? "Category_desc" : "Category";
                ViewBag.DoneSortParm = sortOrder == "Done" ? "Done_desc" : "Done";
                listViewModel.TaskLists = await _context.TaskList.ToListAsync();
                if (!String.IsNullOrEmpty(searchString))
                {
                    listViewModel.TaskLists = listViewModel.TaskLists.Where(s => s.Title.ToUpper().Contains(searchString.ToUpper())).ToList();
                }
                //listViewModel.Categories = _context.Category.OrderBy(x => x.CategoryName).Select(x => new SelectListItem(x.CategoryName, x.CategoryId.ToString())).ToList();
                foreach (var item in listViewModel.TaskLists)
                {
                    item.Tasks = _context.Task.Where(x => x.TaskListId == item.TaskListId).ToList();
                    item.Category = _context.Category.Single(x => x.CategoryId == item.CategoryId);
                }
                switch (sortOrder)
                {
                    case "Title_desc":
                        listViewModel.TaskLists = listViewModel.TaskLists.OrderByDescending(s => s.Title).ToList();
                        break;
                    case "Category":
                        listViewModel.TaskLists = listViewModel.TaskLists.OrderBy(s => s.Category.CategoryName).ToList();
                        break;
                    case "Category_desc":
                        listViewModel.TaskLists = listViewModel.TaskLists.OrderByDescending(s => s.Category.CategoryName).ToList();
                        break;
                    case "CreateDate":
                        listViewModel.TaskLists = listViewModel.TaskLists.OrderBy(s => s.CreateDate).ToList();
                        break;
                    case "CreateDate_desc":
                        listViewModel.TaskLists = listViewModel.TaskLists.OrderByDescending(s => s.CreateDate).ToList();
                        break;
                    case "Done":
                        listViewModel.TaskLists = listViewModel.TaskLists.OrderBy(s => ((float)s.Tasks.Count(x => x.IsDone == true) == 0 && (float)s.Tasks.Count() == 0) ? 0 : (int)Math.Round((float)s.Tasks.Count(x => x.IsDone == true) / (float)s.Tasks.Count() * 100f)).ToList();
                        break;
                    case "Done_desc":
                        listViewModel.TaskLists = listViewModel.TaskLists.OrderByDescending(s => ((float)s.Tasks.Count(x => x.IsDone == true) == 0 && (float)s.Tasks.Count() == 0) ? 0 : (int)Math.Round((float)s.Tasks.Count(x => x.IsDone == true) / (float)s.Tasks.Count() * 100f)).ToList();
                        break;
                    default:
                        listViewModel.TaskLists = listViewModel.TaskLists.OrderBy(s => s.Title).ToList();
                        break;
                }
                return View(listViewModel);
            }
            else
            {
                return Problem("Entity set 'ApplicationDbContext.TaskList'  is null.");
            }

        }

        // GET: TaskLists/Details/5
        public async Task<IActionResult> Details(int? id, string? sortOrder, string? searchString, int? page = 0)
        {
            if (id == null || _context.TaskList == null)
            {
                return NotFound();
            }

            var taskList = await _context.TaskList
                .FirstOrDefaultAsync(m => m.TaskListId == id);

            if (taskList == null)
            {
                return NotFound();
            }
            TaskListViewModel taskListViewModel = new TaskListViewModel();

            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "Title_desc" : "";
            ViewBag.DeadlineSortParm = sortOrder == "Deadline" ? "Deadline_desc" : "Deadline";
            ViewBag.PrioritySortParm = sortOrder == "Priority" ? "Priority_desc" : "Priority";

            taskListViewModel.TaskList = taskList;
            int pageSize = 15;
            var tasks = _context.Task.Where(x => x.TaskListId == id).ToList();
            


            //taskListViewModel.Tasks = _context.Task.Where(x => x.TaskListId == id).ToList();
            //taskListViewModel.Priorities = _context.Priority.OrderBy(x => x.PriorityName).Select(x => new SelectListItem(x.PriorityName, x.PriorityId.ToString())).ToList();
            taskListViewModel.TaskList.Category = _context.Category.Single(x => x.CategoryId == taskList.CategoryId);
            int progress = ((float)tasks.Count(x => x.IsDone == true) == 0 && (float)tasks.Count() == 0) ? 0 : (int)Math.Round((float)tasks.Count(x => x.IsDone == true) / (float)tasks.Count() * 100f);

            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => s.Title.ToUpper().Contains(searchString.ToUpper())).ToList();

            }
            taskListViewModel.progress = progress;
            foreach (var item in tasks)
            {
                item.Priority = _context.Priority.Single(x => x.PriorityId == item.PriorityId);
            }

            switch (sortOrder)
            {
                case "Title_desc":
                    tasks = tasks.OrderByDescending(s => s.Title).OrderBy(s => s.IsDone).ToList();
                    break;
                case "Deadline":
                    tasks = tasks.OrderBy(s => s.Deadline).OrderBy(s => s.IsDone).ToList();
                    break;
                case "Deadline_desc":
                    tasks = tasks.OrderByDescending(s => s.Deadline).OrderBy(s => s.IsDone).ToList();
                    break;
                case "Priority":
                    tasks = tasks.OrderBy(s => s.Priority.PriorityName).OrderBy(s => s.IsDone).ToList();
                    break;
                case "Priority_desc":
                    tasks = tasks.OrderByDescending(s => s.Priority.PriorityName).OrderBy(s => s.IsDone).ToList();
                    break;
                default:
                    tasks = tasks.OrderBy(s => s.Title).OrderBy(s => s.IsDone).ToList();
                    break;
            }
            int totalItems = tasks.Count();
            int totalPages = totalItems / pageSize;
            var pagedTasks = tasks.Skip((int)((int)pageSize * page)).Take(pageSize).ToList();
            taskListViewModel.Tasks = new PaginationViewModel<Models.Task>
            {
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = pagedTasks,
                CurrentPage = (int)page
            };
            return View(taskListViewModel);
        }

        // GET: TaskLists/Create
        public IActionResult Create()
        {
            TaskListViewModel tasklistViewModel = new TaskListViewModel();
            tasklistViewModel.Categories = _context.Category.OrderBy(x => x.CategoryName).Select(x => new SelectListItem(x.CategoryName, x.CategoryId.ToString())).ToList();
            return View(tasklistViewModel);
        }

        // POST: TaskLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskListViewModel taskListViewModel)
        {
            if (ModelState.IsValid)
            {
                TaskList newtasklist = new TaskList
                {
                    Title = taskListViewModel.TaskList.Title,
                    Description = taskListViewModel.TaskList.Description,
                    CreateDate = DateTime.Now,
                    Category = _context.Category.Single(x => x.CategoryId == taskListViewModel.CategoryId)
                };
                _context.Add(newtasklist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            taskListViewModel.Categories = _context.Category.OrderBy(x => x.CategoryName).Select(x => new SelectListItem(x.CategoryName, x.CategoryId.ToString())).ToList();
            return View(taskListViewModel);
        }

        // GET: TaskLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TaskList == null)
            {
                return NotFound();
            }

            var taskList = await _context.TaskList.FindAsync(id);
            if (taskList == null)
            {
                return NotFound();
            }
            TaskListViewModel taskListViewModel = new TaskListViewModel();
            taskList.Category = _context.Category.Single(x => x.CategoryId == taskList.CategoryId);
            taskListViewModel.TaskList = taskList;
            taskListViewModel.CategoryId = taskList.CategoryId;
            taskListViewModel.Categories = _context.Category.OrderBy(x => x.CategoryName).Select(x => new SelectListItem(x.CategoryName, x.CategoryId.ToString())).ToList();
            return View(taskListViewModel);
        }

        // POST: TaskLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskListViewModel taskListViewModel)
        {
            if (!_context.TaskList.Any(x => x.TaskListId == id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTaskList = await _context.TaskList.FindAsync(taskListViewModel.TaskList.TaskListId);
                    existingTaskList.Title = taskListViewModel.TaskList.Title;
                    existingTaskList.Description = taskListViewModel.TaskList.Description;
                    existingTaskList.Category = _context.Category.Single(x => x.CategoryId == taskListViewModel.CategoryId);
                    _context.Update(existingTaskList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskListExists(taskListViewModel.TaskList.TaskListId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "TaskLists", new { id });
            }
            taskListViewModel.Categories = _context.Category.OrderBy(x => x.CategoryName).Select(x => new SelectListItem(x.CategoryName, x.CategoryId.ToString())).ToList();
            return View(taskListViewModel);
        }

        //GET: TaskLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TaskList == null)
            {
                return NotFound();
            }

            var taskList = await _context.TaskList
                .FirstOrDefaultAsync(m => m.TaskListId == id);
            if (taskList == null)
            {
                return NotFound();
            }

            return View(taskList);
        }

        // POST: TaskLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TaskList == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TaskList'  is null.");
            }
            var taskList = await _context.TaskList.FindAsync(id);
            if (taskList != null)
            {
                _context.TaskList.Remove(taskList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskListExists(int id)
        {
            return (_context.TaskList?.Any(e => e.TaskListId == id)).GetValueOrDefault();
        }

    }
}
