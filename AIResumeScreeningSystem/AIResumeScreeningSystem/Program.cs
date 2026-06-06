using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Helpers;
using AIResumeScreeningSystem.Middleware;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Implementations;
using AIResumeScreeningSystem.Repositories.Interfaces;
using AIResumeScreeningSystem.Services.Implementations;
using AIResumeScreeningSystem.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

// ═══════════════════════════════════════════════════════════════════════════
// BOOTSTRAP SERILOG (before builder)
// ═══════════════════════════════════════════════════════════════════════════
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting AI Resume Screening System...");

    var builder = WebApplication.CreateBuilder(args);

    // ════════════════════════════════════════════════════════════════════════
    // SERILOG
    // ════════════════════════════════════════════════════════════════════════
    builder.Host.UseSerilog((ctx, services, config) => config
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // ════════════════════════════════════════════════════════════════════════
    // DATABASE — Entity Framework Core + SQL Server
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(60);
            }));

    // ════════════════════════════════════════════════════════════════════════
    // ASP.NET CORE IDENTITY
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        // Lockout
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    // ════════════════════════════════════════════════════════════════════════
    // COOKIE CONFIGURATION
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

    // ════════════════════════════════════════════════════════════════════════
    // AUTHORIZATION POLICIES
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly",
            policy => policy.RequireRole("Admin"));
        options.AddPolicy("RecruiterOnly",
            policy => policy.RequireRole("Recruiter"));
        options.AddPolicy("CandidateOnly",
            policy => policy.RequireRole("Candidate"));
        options.AddPolicy("AdminOrRecruiter",
            policy => policy.RequireRole("Admin", "Recruiter"));
    });

    // ════════════════════════════════════════════════════════════════════════
    // MVC + RAZOR VIEWS
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddControllersWithViews()
        .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore);

    builder.Services.AddRazorPages();

    // ════════════════════════════════════════════════════════════════════════
    // SESSION
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    builder.Services.AddHttpContextAccessor();

    // ════════════════════════════════════════════════════════════════════════
    // AUTOMAPPER
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

    // ════════════════════════════════════════════════════════════════════════
    // FLUENT VALIDATION
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // ════════════════════════════════════════════════════════════════════════
    // HTTP CLIENT — OpenAI / Anthropic
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddHttpClient("OpenAI", client =>
    {
        client.Timeout = TimeSpan.FromSeconds(90);
        client.BaseAddress = new Uri("https://api.anthropic.com");
    });

    builder.Services.AddHttpClient();

    // ════════════════════════════════════════════════════════════════════════
    // FILE UPLOAD SIZE LIMITS
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
    });

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
    });

    // ════════════════════════════════════════════════════════════════════════
    // ── STEP 2: AUTHENTICATION SERVICES ────────────────────────────────────
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IEmailService, EmailService>();

    // ════════════════════════════════════════════════════════════════════════
    // ── STEP 3: JOB MODULE ─────────────────────────────────────────────────
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddScoped<IJobRepository, JobRepository>();
    builder.Services.AddScoped<IJobService, JobService>();

    // ════════════════════════════════════════════════════════════════════════
    // ── STEP 4: CANDIDATE MODULE ────────────────────────────────────────────
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
    builder.Services.AddScoped<ICandidateService, CandidateService>();

    // ════════════════════════════════════════════════════════════════════════
    // ── STEP 5: RESUME MODULE ──────────────────────────────────────────────
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddScoped<IResumeRepository, ResumeRepository>();
    builder.Services.AddScoped<IResumeParserService, ResumeParserService>();
    builder.Services.AddScoped<IResumeService, ResumeService>();

    // ════════════════════════════════════════════════════════════════════════
    // ── STEP 6: SKILL MATCHING MODULE ──────────────────────────────────────
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
    builder.Services.AddScoped<ISkillMatchingService, SkillMatchingService>();
    builder.Services.AddScoped<ICandidateRankingService, CandidateRankingService>();

    // ════════════════════════════════════════════════════════════════════════
    // ── STEP 7: OPENAI MODULE ──────────────────────────────────────────────
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddScoped<IOpenAIService, OpenAIService>();

    // ════════════════════════════════════════════════════════════════════════
    // ── STEP 8: DASHBOARD MODULE ───────────────────────────────────────────
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddScoped<IDashboardService, DashboardService>();

    // ════════════════════════════════════════════════════════════════════════
    // ── STEP 9: NOTIFICATION + REPORT MODULE ───────────────────────────────
    // ════════════════════════════════════════════════════════════════════════
    builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<IReportService, ReportService>();

    // ════════════════════════════════════════════════════════════════════════
    // BUILD APP
    // ════════════════════════════════════════════════════════════════════════
    var app = builder.Build();

    // ════════════════════════════════════════════════════════════════════════
    // MIDDLEWARE PIPELINE
    // ════════════════════════════════════════════════════════════════════════
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    // Global exception handler (custom middleware)
    app.UseGlobalExceptionHandler();

    // Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} " +
            "in {Elapsed:0.0000}ms";
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseSession();
    app.UseAuthentication();
    app.UseAuthorization();

    // ════════════════════════════════════════════════════════════════════════
    // ROUTE CONFIGURATION
    // ════════════════════════════════════════════════════════════════════════
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();

    // ════════════════════════════════════════════════════════════════════════
    // DATABASE MIGRATION & SEEDING
    // ════════════════════════════════════════════════════════════════════════
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();

            logger.LogInformation("Seeding roles and admin user...");
            await SeedRolesAndAdminAsync(userManager, roleManager, logger);

            logger.LogInformation("Database ready.");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogCritical(ex, "A critical error occurred during startup.");
            throw;
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // ENSURE UPLOAD DIRECTORIES EXIST
    // ════════════════════════════════════════════════════════════════════════
    EnsureUploadDirectories(app);

    Log.Information("AI Resume Screening System started successfully.");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

// ═══════════════════════════════════════════════════════════════════════════
// SEED METHOD
// ═══════════════════════════════════════════════════════════════════════════
static async Task SeedRolesAndAdminAsync(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<Program> logger)
{
    string[] roles = { "Admin", "Recruiter", "Candidate" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            logger.LogInformation("Created role: {Role}", role);
        }
    }

    const string adminEmail = "admin@airesumescreen.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "System",
            LastName = "Admin",
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin, "Admin@12345");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
            logger.LogInformation("Default admin created: {Email}", adminEmail);
        }
        else
        {
            foreach (var error in result.Errors)
                logger.LogWarning("Admin seed error: {Error}", error.Description);
        }
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// ENSURE UPLOAD DIRECTORIES
// ═══════════════════════════════════════════════════════════════════════════
static void EnsureUploadDirectories(WebApplication app)
{
    var env = app.Services.GetRequiredService<IWebHostEnvironment>();
    var folders = new[]
    {
        Path.Combine(env.WebRootPath, "uploads", "resumes"),
        Path.Combine(env.WebRootPath, "uploads", "profiles"),
        Path.Combine(env.WebRootPath, "uploads", "reports"),
        Path.Combine(env.ContentRootPath, "Logs")
    };

    foreach (var folder in folders)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            Log.Information("Created directory: {Folder}", folder);
        }
    }
}