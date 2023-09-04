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
                Category studia = new Category { CategoryName = "Studia" };

                TaskList l1 = new TaskList { Title = "Projekt ToDo", CategoryId = 4, Category = studia, CreateDate = DateTime.Now, User=context.Users.Single(x=>x.Email=="user@user.com")};
                TaskList l2 = new TaskList { Title = "Testowa lista", CategoryId = 1, Category = praca, CreateDate = DateTime.Now, User = context.Users.Single(x => x.Email == "user@user.com") };

                Task z1 = new Task { Title = "Zadanie 1", Description = "Opis", TaskListId = 2, TaskList = l2, PriorityId = 2, Priority = wazne };
                Task z2 = new Task { Title = "Zadanie 2", Description = "Opis", TaskListId = 2, TaskList = l2, PriorityId = 1, Priority = pilne };
                Task z3 = new Task { Title = "Zadanie 3", Description = "Opis", TaskListId = 2, TaskList = l2, PriorityId = 3, Priority = niski };

                if (!context.Priority.Any())
                {
                    context.Priority.AddRange(pilne, wazne, niski);
                }

                if (!context.Category.Any())
                {
                    context.Category.AddRange(praca, dom, zakupy, studia);
                }

                if (!context.TaskList.Any())
                {
                    context.TaskList.AddRange(l1, l2);
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
