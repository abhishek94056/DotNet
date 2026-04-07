using InvoiceGenerator.Services;
using InvoiceGenerator.Interfaces;

namespace InvoiceGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Builder -> Used to configure services and app settings (Setup Phase)

            // ── Services (Dependency Injection Container) ──
            builder.Services.AddControllersWithViews();
            // Enables MVC (Model-View-Controller) support

            // ── Session Configuration ──
            builder.Services.AddDistributedMemoryCache();
            // Stores session data in server memory

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                // Session expires after 60 minutes of inactivity

                options.Cookie.HttpOnly = true;
                // Prevents client-side JavaScript access (security)

                options.Cookie.IsEssential = true;
                // Required for GDPR (session always enabled)

                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                // Uses HTTPS if request is HTTPS
            });

            // ── Custom Services (Dependency Injection) ──
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<PdfService>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<ITransportService, TransportService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            // AddScoped -> One instance per HTTP request

            var app = builder.Build();
            // Builds the application pipeline

            // ── Middleware Pipeline ──
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // Redirects to error page in production

                app.UseHsts();
                // Forces HTTPS for security (HTTP Strict Transport Security)
            }

            app.UseHttpsRedirection();
            // Redirect HTTP requests to HTTPS

            app.UseStaticFiles();
            // Enables serving static files (CSS, JS, images)

            app.UseRouting();
            // Matches incoming request URL to endpoints

            app.UseSession();
            // Enables session (⚠ must come before endpoints)

            app.UseAuthorization();
            // Handles authorization (role/security checks)

            // ── Attribute Routing ──
            app.MapControllers();
            // Enables attribute-based routing

            // ── Conventional Routing ──
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Invoice}/{action=InvoiceView}/{id?}");

            app.Run();
            // Starts the application
        }
    }
}