// Models/SupplierModel.cs
using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    public class SupplierModel
    {
        public int SupplierId { get; set; }
        public string? Supplier { get; set; }
        public int? IsActive { get; set; }
        public string? Address { get; set; }
        public int? StateCode { get; set; }
        public string? State { get; set; }
        public string? GSTIN { get; set; }
        public string? CreatedBy { get; set; }

    }
}