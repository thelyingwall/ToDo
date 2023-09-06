using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.Models;
using ToDo.ViewModels;

namespace ToDo.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TasksController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
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
            IdentityUser user = await _userManager.GetUserAsync(User);
            task.TaskList = _context.TaskList.Single(x => x.TaskListId == task.TaskListId);
            if (!_context.TaskList.Any(utl => utl.User.Id == user.Id && utl.TaskListId == task.TaskList.TaskListId))
            {
                return NotFound();
            }

            task.Priority = _context.Priority.Single(x => x.PriorityId == task.PriorityId);
            Console.WriteLine(task.ToString());
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
            taskViewModel.Priorities = _context.Priority.OrderBy(x => x.PriorityName).Select(x => new SelectListItem(x.PriorityName, x.PriorityId.ToString())).ToList();
            taskViewModel.TaskListId = taskListId;
            return View(taskViewModel);
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel taskViewModel, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {

                Models.Task newtask = new Models.Task
                {
                    Title = taskViewModel.Task.Title,
                    Description = taskViewModel.Task.Description,
                    IsDone = false,
                    CreateDate = DateTime.Now,
                    Deadline = taskViewModel.Deadline,
                    ImagePath = imageFile != null ? await SaveImage(imageFile) : null,
                    TaskList = _context.TaskList.Single(x => x.TaskListId == taskViewModel.TaskListId),
                    Priority = _context.Priority.Single(x => x.PriorityId == taskViewModel.PriorityId)

                };
                _context.Task.Add(newtask);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "TaskLists", new { id = taskViewModel.TaskListId });
            }
            taskViewModel.Priorities = _context.Priority.OrderBy(x => x.PriorityName).Select(x => new SelectListItem(x.PriorityName, x.PriorityId.ToString())).ToList();
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
            
            TaskViewModel taskViewModel = new TaskViewModel();
            IdentityUser user = await _userManager.GetUserAsync(User);
            task.TaskList = _context.TaskList.Single(x => x.TaskListId == task.TaskListId);
            if (!_context.TaskList.Any(utl => utl.User.Id == user.Id && utl.TaskListId == task.TaskList.TaskListId))
            {
                return NotFound();
            }
            task.Priority = _context.Priority.Single(x => x.PriorityId == task.PriorityId);
            taskViewModel.Task = task;
            taskViewModel.PreviousImagePath = task.ImagePath;
            taskViewModel.TaskListId = task.TaskListId;
            taskViewModel.Deadline = task.Deadline;
            taskViewModel.PriorityId = task.PriorityId;
            taskViewModel.Priorities = _context.Priority.OrderBy(x => x.PriorityName).Select(x => new SelectListItem(x.PriorityName, x.PriorityId.ToString())).ToList();
            Console.WriteLine(taskViewModel.Task.ToString());
            return View(taskViewModel);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskViewModel taskViewModel, IFormFile? imageFile)
        {
            if (!_context.Task.Any(x => x.TaskId == id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTask = await _context.Task.FindAsync(id);
                    existingTask.Priority = _context.Priority.Single(x => x.PriorityId == taskViewModel.PriorityId);

                    existingTask.Title = taskViewModel.Task.Title;
                    existingTask.Description = taskViewModel.Task.Description;
                    existingTask.IsDone = taskViewModel.Task.IsDone;
                    existingTask.Deadline = taskViewModel.Deadline;
                    if (imageFile != null)
                    {
                        existingTask.ImagePath = await SaveImage(imageFile);
                    }
                    else if (!string.IsNullOrEmpty(taskViewModel.PreviousImagePath))
                    {
                        existingTask.ImagePath = taskViewModel.PreviousImagePath;
                    }
                    _context.Update(existingTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(taskViewModel.Task.TaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Tasks", new { id });
            }
            taskViewModel.Priorities = _context.Priority.OrderBy(x => x.PriorityName).Select(x => new SelectListItem(x.PriorityName, x.PriorityId.ToString())).ToList();
            return View(taskViewModel);
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
            IdentityUser user = await _userManager.GetUserAsync(User);
            task.TaskList = _context.TaskList.Single(x => x.TaskListId == task.TaskListId);
            if (!_context.TaskList.Any(utl => utl.User.Id == user.Id && utl.TaskListId == task.TaskList.TaskListId))
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
            return RedirectToAction("Details", "TaskLists", new { id = task.TaskListId });
        }

        private bool TaskExists(int id)
        {
            return (_context.Task?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }

        [HttpPost, ActionName("Check")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Check(int id)
        {
            if (!_context.Task.Any(x => x.TaskId == id))
            {
                return NotFound();
            }
            var task = await _context.Task.FindAsync(id);
            if (task != null)
            {
                task.IsDone = !task.IsDone;
                _context.Update(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "TaskLists", new { id = task.TaskListId });
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                return "/images/" + imageName;
            }
            return null;
        }
    }
}
