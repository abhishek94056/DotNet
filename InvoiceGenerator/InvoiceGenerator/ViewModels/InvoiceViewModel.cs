using InvoiceGenerator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace InvoiceGenerator.ViewModels
{
    // ViewModels/InvoiceViewModel.cs
    public class InvoiceViewModel
    {
        public InvoiceMaster Master { get; set; } = new();
        public List<InvoiceItem> Items { get; set; } = new();

        // Dropdowns
        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> TransportModes { get; set; } = new();
        public List<SelectListItem> ItemsList { get; set; } = new();

        // Display
        public string NextInvoiceLabel { get; set; }
        public CompanyDetails InvoiceToDetails { get; set; }
        public CompanyDetails ShippingToDetails { get; set; }
    }
}
