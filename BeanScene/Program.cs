using BeanScene.Components;
using BeanScene.Data;
using BeanScene.Models;
using BeanScene.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentInfoLoginRoles.Hubs;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.AddScoped<EmailSender>();

builder.Services.AddScoped<
    Microsoft.AspNetCore.Identity.UI.Services.IEmailSender,
    IdentityEmailSender>();


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Manager", "Staff", "Member" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var user = await userManager.FindByEmailAsync("atika@test.com");

    if (user != null && !await userManager.IsInRoleAsync(user, "Manager"))
    {
        await userManager.AddToRoleAsync(user, "Manager");
    }
}

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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();





app.MapRazorComponents<App>();

app.MapRazorPages()
   .WithStaticAssets();

app.MapRazorComponents<ImageGenerator>()
    .AddInteractiveServerRenderMode();


app.MapHub<ChatHub>("/chathub");

app.MapControllers();


app.Run();
