using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.Models;

namespace ToDo
{
    public class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=TaskLists}/{action=Index}/{id?}");
            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roles = new[] { "admin", "user" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                string emailAdmin = "admin@admin.com";
                string passwordAdmin = "Haslo123.";
                
                if (await userManager.FindByEmailAsync(emailAdmin) == null)
                {
                    var userAdmin = new IdentityUser();
                    userAdmin.UserName = emailAdmin;
                    userAdmin.Email = emailAdmin;

                    var createUserResult = await userManager.CreateAsync(userAdmin, passwordAdmin);
                    if (createUserResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(userAdmin, "admin");
                    }
                }
                string emailUser = "user@user.com";
                string passwordUser = "Haslo123.";

                if (await userManager.FindByEmailAsync(emailUser) == null)
                {
                    var user = new IdentityUser();
                    user.UserName = emailUser;
                    user.Email = emailUser;

                    var createUserResult = await userManager.CreateAsync(user, passwordUser);
                    if (createUserResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "user");
                    }
                }
                var services = scope.ServiceProvider;

                SeedData.Initialize(services);
            }
            app.Run();
        }
    }
}