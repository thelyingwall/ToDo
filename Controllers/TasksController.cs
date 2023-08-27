using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.Models;
using ToDo.ViewModels;

namespace ToDo.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
              return _context.Task != null ? 
                          View(await _context.Task.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Task'  is null.");
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Task == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create(int taskListId)
        {
            var taskList = _context.TaskList.FirstOrDefault(tl => tl.TaskListId == taskListId);

            if (taskList == null)
            {
                return NotFound();
            }

            TaskViewModel taskViewModel = new TaskViewModel();
            taskViewModel.Categories = _context.Category.Select(x => new SelectListItem(x.CategoryName, x.CategoryId.ToString())).ToList();
            taskViewModel.Priorities = _context.Priority.Select(x => new SelectListItem(x.PriorityName, x.PriorityId.ToString())).ToList();
            taskViewModel.TaskListId = taskListId;
            return View(taskViewModel);
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel taskViewModel)
        {
            if (ModelState.IsValid)
            {
                //TaskList taskList = _context.TaskList.Single(x => x.TaskListId == taskViewModel.TaskListId);
                //Category category = _context.Category.Single(x => x.CategoryId == taskViewModel.CategoryId);
                //Priority priority = _context.Priority.Single(x => x.PriorityId == taskViewModel.PriorityId);

                Models.Task newtask = new Models.Task
                {
                    Title = taskViewModel.Task.Title,
                    Description = taskViewModel.Task.Description,
                    IsDone = false,
                    CreateDate = DateTime.Now,
                    Deadline = taskViewModel.Task.Deadline,
                    TaskList = _context.TaskList.Single(x => x.TaskListId == taskViewModel.TaskListId),
                    Category = _context.Category.Single(x => x.CategoryId == taskViewModel.CategoryId),
                    Priority = _context.Priority.Single(x => x.PriorityId == taskViewModel.PriorityId)

                };
                Console.WriteLine(newtask.ToString());
                _context.Task.Add(newtask);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "TaskLists", new { id = taskViewModel.TaskListId });
            }
            return View(taskViewModel);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Task == null)
            {
                return NotFound();
            }

            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,Title,Description,IsDone,Deadline")] ToDo.Models.Task task)
        {
            if (id != task.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTaskList = await _context.Task.FindAsync(task.TaskId);
                    task.CreateDate = existingTaskList.CreateDate; 
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.TaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Task == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Task == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Task'  is null.");
            }
            var task = await _context.Task.FindAsync(id);
            if (task != null)
            {
                _context.Task.Remove(task);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
          return (_context.Task?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }
    }
}
