using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.Models;
using ToDo.ViewModels;

namespace ToDo.Controllers
{
    [Authorize(Roles = "admin")]
    public class PrioritiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrioritiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Priorities
        public async Task<IActionResult> Index(string? sortOrder, string? searchString)
        {
            if (_context.Priority != null)
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
                List<Priority> priorityList = await _context.Priority.ToListAsync();
                if (!String.IsNullOrEmpty(searchString))
                {
                    priorityList = priorityList.Where(s => s.PriorityName.ToUpper().Contains(searchString.ToUpper())).ToList();
                }
                switch (sortOrder)
                {
                    case "Name_desc":
                        priorityList = priorityList.OrderByDescending(s => s.PriorityName).ToList();
                        break;
                    default:
                        priorityList = priorityList.OrderBy(s => s.PriorityName).ToList();
                        break;
                }
                return View(priorityList);
            }
            else
            {
                return Problem("Entity set 'ApplicationDbContext.Priority'  is null.");
            }
        }

        // GET: Priorities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Priority == null)
            {
                return NotFound();
            }

            var priority = await _context.Priority
                .FirstOrDefaultAsync(m => m.PriorityId == id);
            if (priority == null)
            {
                return NotFound();
            }

            return View(priority);
        }

        // GET: Priorities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Priorities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PriorityId,PriorityName")] Priority priority)
        {
            if (ModelState.IsValid)
            {
                _context.Add(priority);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(priority);
        }

        // GET: Priorities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Priority == null)
            {
                return NotFound();
            }

            var priority = await _context.Priority.FindAsync(id);
            if (priority == null)
            {
                return NotFound();
            }
            return View(priority);
        }

        // POST: Priorities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PriorityId,PriorityName")] Priority priority)
        {
            if (id != priority.PriorityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priority);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriorityExists(priority.PriorityId))
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
            return View(priority);
        }

        // GET: Priorities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Priority == null)
            {
                return NotFound();
            }

            var priority = await _context.Priority
                .FirstOrDefaultAsync(m => m.PriorityId == id);
            if (priority == null)
            {
                return NotFound();
            }

            return View(priority);
        }

        // POST: Priorities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Priority == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Priority'  is null.");
            }
            var priority = await _context.Priority.FindAsync(id);
            if (priority != null)
            {
                _context.Priority.Remove(priority);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriorityExists(int id)
        {
            return (_context.Priority?.Any(e => e.PriorityId == id)).GetValueOrDefault();
        }
    }
}
