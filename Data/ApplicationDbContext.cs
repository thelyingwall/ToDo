using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDo.Models;

namespace ToDo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ToDo.Models.Category>? Category { get; set; }
        public DbSet<ToDo.Models.Comment>? Comment { get; set; }
        public DbSet<ToDo.Models.Priority>? Priority { get; set; }
        public DbSet<ToDo.Models.Task>? Task { get; set; }
        public DbSet<ToDo.Models.TaskList>? TaskList { get; set; }
    }
}