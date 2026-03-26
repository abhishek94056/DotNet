using InvoiceGenerator.Services;

namespace InvoiceGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── Services ──────────────────────────────────────────────
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<InvoiceService>();
            builder.Services.AddScoped<PdfService>();

            var app = builder.Build();

            // ── Middleware Pipeline ───────────────────────────────────
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Invoice/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // ── Routes ────────────────────────────────────────────────
            app.MapControllerRoute(
                name: "pdf",
                pattern: "Invoice/Pdf/{invoiceNo:int}");
                //defaults: new { controller = "Invoice", action = "Pdf" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Invoice}/{action=Create}/{id?}");

            app.Run();
        }
    }
}