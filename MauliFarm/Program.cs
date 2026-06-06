using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MauliFarm.Data;
using MauliFarm.Models;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Database ───────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MauliFarmConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }
    )
);

// ── 2. Identity ───────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit           = true;
    options.Password.RequireLowercase       = true;
    options.Password.RequireUppercase       = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength         = 6;

    options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers      = true;

    options.User.RequireUniqueEmail         = true;
    options.SignIn.RequireConfirmedEmail     = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ── 3. Cookie ─────────────────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath          = "/Auth/Login";
    options.LogoutPath         = "/Auth/Logout";
    options.AccessDeniedPath   = "/Auth/AccessDenied";
    options.ExpireTimeSpan     = TimeSpan.FromHours(8);
    options.SlidingExpiration  = true;
    options.Cookie.HttpOnly    = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite    = SameSiteMode.Lax;
    options.Cookie.Name        = "MauliFarm.Session";
});

// ── 4. MVC ────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// ── 5. Session ────────────────────────────────────────────────────────────────
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

await DbInitializer.InitializeAsync(app.Services);

app.Run();
