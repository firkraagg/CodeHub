using CodeHub.Components;
using CodeHub.Data;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CodeHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new NullReferenceException("No connection string in config!");

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
            var connectionString = $"Data Source={dbHost},1433;Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=True;";
            builder.Services.AddDbContextFactory<DatabaseContext>((DbContextOptionsBuilder options) => options.UseSqlServer(connectionString));
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                dbContext.Database.Migrate();
            }
            builder.Services.AddTransient<UserService>();
            builder.Services.AddTransient<ProblemService>();
            builder.Services.AddTransient<ProgrammingLanguageService>();
            builder.Services.AddTransient<TagService>();
            builder.Services.AddTransient<PistonService>();
            builder.Services.AddTransient<ProblemHintService>();
            builder.Services.AddTransient<ProblemConstraintService>();
            builder.Services.AddTransient<ProblemExampleService>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
