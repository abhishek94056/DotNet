using InvoiceGenerator.Services;
using InvoiceGenerator.Interfaces;

namespace InvoiceGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);     //Builder (Setup Phase)

            // ── Services ──────────────────────────────────────────────
            builder.Services.AddControllersWithViews();         //(Model-View-Controller) enable
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();       //Custom Services (Dependency Injection) 
            builder.Services.AddScoped<PdfService>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<ITransportService, TransportService>();
            var app = builder.Build();                          //Build Application

            // ── Middleware Pipeline ───────────────────────────────────
            if (!app.Environment.IsDevelopment())              //Middleware Pipeline(Production environment check)
            {
                app.UseExceptionHandler("/Invoice/Error");    //Error Handling(if error occure redirect to /Invoice/Error)
                app.UseHsts();    //enforce to use HTTPS not HTTP
            }

            app.UseHttpsRedirection(); // HTTP → HTTPS
            app.UseStaticFiles();      // CSS, JS, images
            app.UseRouting();          // URL → Controller mapping
            app.UseAuthorization();    // Security check

            // ── Attribute Routing ────────────────────────────────────────────────
            app.MapControllers();

            // ── Conventional Routing ────────────────────────────────────────────────
            //app.MapControllerRoute(
            //    name: "pdf",
            //    pattern: "Invoice/Pdf/{invoiceNo:int}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Invoice}/{action=InvoiceView}/{id?}");

            app.Run();   //Run Application
        }
    }
}