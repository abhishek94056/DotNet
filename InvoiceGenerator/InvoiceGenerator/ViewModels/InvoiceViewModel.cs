using InvoiceGenerator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace InvoiceGenerator.ViewModels
{
    // ViewModels/InvoiceViewModel.cs
    public class InvoiceViewModel
    {
        public InvoiceModel Master { get; set; } = new();
        public List<ItemModel> Items { get; set; } = new();

        // Dropdowns
        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> TransportModes { get; set; } = new();
        public List<SelectListItem> ItemsList { get; set; } = new();

        // Display
        public string NextInvoiceLabel { get; set; }
        public CompanyModel InvoiceToDetails { get; set; }
        public CompanyModel ShippingToDetails { get; set; }
    }
}
