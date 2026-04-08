using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using WebChat.Components;
using WebChat.Components.Account;
using WebChat.Components.Account.Hubs;
using WebChat.Components.Account.Models;
using WebChat.Data;

namespace WebChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents()
                .AddAuthenticationStateSerialization();

            builder.Services.AddSignalR();


            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
            builder.Services.AddHttpClient("api", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7067");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new CookieContainer()
                };
            });

            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7067")
    });


            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            var app = builder.Build();

            app.MapHub<ChatHub>("/chatHub");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapPost("/api/login", async (
    HttpContext context,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    LoginModel model) =>
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                    return Results.BadRequest("Invalid credentials");

                var result = await signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    isPersistent: true,
                    lockoutOnFailure: false
                );

                if (!result.Succeeded)
                    return Results.BadRequest("Invalid credentials");

                await context.SignInAsync(
                    IdentityConstants.ApplicationScheme,
                    new ClaimsPrincipal(
                        await signInManager.CreateUserPrincipalAsync(user)
                    )
                );

                return Results.Ok();
            });

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.MapPost("/api/register", async (
    UserManager<ApplicationUser> userManager,
    RegisterModel model) =>
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return Results.BadRequest(result.Errors.Select(e => e.Description));

                return Results.Ok();
            });

            app.MapGet("/api/chats/{userId}", async (ApplicationDbContext db, string userId) =>
            {
                return await db.Chats
                    .Include(c => c.Users)
                    .Where(c => c.Users.Any(u => u.Id == userId))
                    .ToListAsync();
            });

            app.MapGet("/api/messages/{chatId}", async (ApplicationDbContext db, int chatId) =>
            {
                return await db.Messages
                    .Where(m => m.ChatId == chatId)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();
            });

            app.Run();
        }
    }
}