using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using ToDo.Data;
using ToDo.Models;
using ToDo.ViewModels;

namespace ToDo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> StatisticsAsync()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            var tasks = _context.Task.Include(t => t.Priority).Where(t => t.TaskList.User.Id == user.Id).ToList();

            var taskDoneOrNot = tasks
                .GroupBy(t => t.IsDone)
                .Select(g => new StatisticViewModel
                {
                    Key = g.Key == true ? "Wykonano" : "Nie wykonano",
                    Value = g.Count()
                })
                .ToList();

            var taskCountByPriority = tasks
                .Where(t => t.Priority != null)
                .GroupBy(t => t.Priority.PriorityName)
                .Select(g => new StatisticViewModel
                {
                    Key = g.Key,
                    Value = g.Count()
                })
                .ToList();

            var viewModel = new StatisticsListViewModel
            {
                stat1 = taskCountByPriority,
                stat2 = taskDoneOrNot
            };
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}