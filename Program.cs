using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
builder.Services.AddRazorPages(options =>
{
    // Map the root URL "/" to the Razor Page file at "/Gallery/Index"
    options.Conventions.AddPageRoute("/Gallery", "/");
});

var app = builder.Build();

var supportedCultures = new[]
{
    new CultureInfo("ru-RU"),
    new CultureInfo("en-US")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ru-RU"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

await SeedModeratorAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", context =>
{
    context.Response.Redirect("/Gallery/Index");
    return Task.CompletedTask;
});

app.MapRazorPages();

app.Run();

async Task SeedModeratorAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string moderatorRole = "Moderator";

        if (!await roleManager.RoleExistsAsync(moderatorRole))
            await roleManager.CreateAsync(new IdentityRole(moderatorRole));

        string modEmail = "moderator@local";
        var mod = await userManager.FindByEmailAsync(modEmail);

        if (mod == null)
        {
            mod = new ApplicationUser
            {
                UserName = modEmail,
                Email = modEmail,
                EmailConfirmed = true,
                DisplayName = "Moderator",
            };

            var createResult = await userManager.CreateAsync(mod, "Moderator123!");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(mod, moderatorRole);
                logger.LogInformation("Seeded moderator user.");
            }
            else
            {
                logger.LogWarning("Failed to create moderator user: {0}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(mod);
            await userManager.ResetPasswordAsync(mod, token, "Moderator123!");

            if (!await userManager.IsInRoleAsync(mod, moderatorRole))
                await userManager.AddToRoleAsync(mod, moderatorRole);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the DB.");
    }
}