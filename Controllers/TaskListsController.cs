using System;
using System.Collections.Generic;
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
    //[Authorize]
    public class TaskListsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TaskLists
        public async Task<IActionResult> Index()
        {
            if (_context.TaskList != null)
            {
                ListViewModel listViewModel = new ListViewModel();
                listViewModel.TaskLists = await _context.TaskList.ToListAsync();
                foreach (var item in listViewModel.TaskLists)
                {
                    item.Tasks = _context.Task.Where(x => x.TaskListId == item.TaskListId).ToList();
                    Console.WriteLine(item.Tasks.Count());
                }
                return View(listViewModel);
            }
            else
            {
                return Problem("Entity set 'ApplicationDbContext.TaskList'  is null.");
            }
            
        }

        // GET: TaskLists/Details/5
        public async Task<IActionResult> Details(int? id)
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
            foreach (var item in _context.Task.Where(x => x.TaskListId == id))
            {
                Console.WriteLine(item.Title.ToString());
            }
            taskListViewModel.TaskList = taskList;
            taskListViewModel.Tasks = _context.Task.Where(x => x.TaskListId == id).ToList();
            foreach (var item in taskListViewModel.Tasks)
            {
                item.Priority = _context.Priority.Single(x => x.PriorityId == item.PriorityId);
            }
            return View(taskListViewModel);
        }

        // GET: TaskLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaskLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskListId,Title,Description")] TaskList taskList)
        {
            if (ModelState.IsValid)
            {
                taskList.CreateDate = DateTime.Now;
                _context.Add(taskList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskList);
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
            return View(taskList);
        }

        // POST: TaskLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskListId,Title,Description")] TaskList taskList)
        {
            if (id != taskList.TaskListId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTaskList = await _context.TaskList.FindAsync(taskList.TaskListId);
                    existingTaskList.Title = taskList.Title;
                    existingTaskList.Description = taskList.Description;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskListExists(taskList.TaskListId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "TaskLists", new { id = taskList.TaskListId });
            }
            return View(taskList);
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
