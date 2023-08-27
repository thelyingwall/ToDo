using Microsoft.EntityFrameworkCore;
using ToDo.Data;

namespace ToDo.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {


                Priority pilne = new Priority { PriorityName = "Pilne" };
                Priority wazne = new Priority { PriorityName = "Ważne" };
                Priority niski = new Priority { PriorityName = "Niski priorytet" };

                Category praca = new Category { CategoryName = "Praca" };
                Category dom = new Category { CategoryName = "Dom" };
                Category zakupy = new Category { CategoryName = "Zakupy" };

                TaskList l1 = new TaskList { Title = "Nowa lista zadań" };

                Task z1 = new Task { Title = "Zadanie 1", Description = "Opis", TaskListId=1, TaskList = l1, PriorityId=2, Priority = wazne, CategoryId=1, Category = praca };
                Task z2 = new Task { Title = "Zadanie 2", Description = "Opis", TaskListId = 1, TaskList = l1, PriorityId = 1, Priority = pilne, CategoryId = 2, Category = dom };
                Task z3 = new Task { Title = "Zadanie 3", Description = "Opis", TaskListId = 1, TaskList = l1, PriorityId = 3, Priority = niski, CategoryId = 3, Category = zakupy };

                // Look for any movies.


                if (!context.Priority.Any())
                {
                    context.Priority.AddRange(pilne, wazne, niski);
                }

                if (!context.Category.Any())
                {
                    context.Category.AddRange(praca, dom, zakupy);
                }

                if (!context.TaskList.Any())
                {
                    context.TaskList.AddRange(l1);
                }

                if (!context.Task.Any())
                {
                    context.Task.AddRange(z1, z2, z3);
                }

                context.SaveChanges();
            }
        }
    }
}
