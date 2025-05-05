using BlazorDownloadFile;
using CodeHub.Components;
using CodeHub.Configuration;
using CodeHub.Data;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace CodeHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
             
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

            /// <summary>
            /// T·to metÛda vytv·ra pripojenie k datab·ze pomocou pripojovacieho reùazca.
            /// Lok·lne pripojenie cez tento reùazec funguje bez problÈmov, avöak pri spustenÌ aplik·cie cez Docker sa vyskytuje problÈm s ch˝baj˙cou
            /// kniûnicou 'ldap.so.2', ktor· je potrebn· pre LDAP autentifik·ciu. Tento problem pravdepodobne vznik·, pretoûe ch˝ba podpora pre LDAP v prostredÌ Dockeru (Linux).
            /// </summary>
            //var connectionString = $"Data Source={dbHost},1433;Initial Catalog={dbName}" +
            //    $";User ID=sa;Password={dbPassword};TrustServerCertificate=True;";

            // AlternatÌvny pripojovacÌ reùazec pre lok·lne pripojenie (funguje bez problÈmov)
            var connectionString = "Data Source=localhost,8002;Initial Catalog=CodeHubApp;User ID=sa;Password=CodeHub@2023;TrustServerCertificate=True;";

            builder.Services.AddDbContextFactory<DatabaseContext>((DbContextOptionsBuilder options) => options.UseSqlServer(connectionString));
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                dbContext.Database.Migrate();
            }

            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
                .SetApplicationName("CodeHubApp");  
            builder.Services.AddBlazorDownloadFile();
            builder.Services.AddTransient<UserService>();
            builder.Services.AddTransient<ProblemService>();
            builder.Services.AddTransient<ProgrammingLanguageService>();
            builder.Services.AddTransient<TagService>();
            builder.Services.AddTransient<ProblemHintService>();
            builder.Services.AddTransient<ProblemConstraintService>();
            builder.Services.AddTransient<ProblemExampleService>();
            builder.Services.AddSingleton<RabbitMqProducerService>();
            builder.Services.AddTransient<TestCaseService>();
            builder.Services.AddTransient<ProblemsAttemptService>();
            builder.Services.AddTransient<VisibleWeekService>();
            builder.Services.AddScoped<ProblemCacheService>();
            builder.Services.Configure<LdapSettings>(builder.Configuration.GetSection("LdapSettings"));
            var ldapSettings = builder.Configuration.GetSection("LdapSettings").Get<LdapSettings>();
            builder.Services.AddSingleton<LdapService>(new LdapService("fri.uniza.sk", 389, ldapSettings));
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(300);
                options.Cookie.Name = "CodeHub.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy => policy.RequireClaim("Role", "admin"));
            });
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseRouting();
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