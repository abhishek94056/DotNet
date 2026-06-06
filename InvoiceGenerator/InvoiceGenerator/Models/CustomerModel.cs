using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    public class CustomerModel
    {
        public int CustomerId { get; set; }
        public string? Customer { get; set; }
        public int? IsActive { get; set; }
        public string? Address { get; set; }
        public int? StateCode { get; set; }
        public string? State { get; set; }
        public string? GSTIN { get; set; }
        public string? CreatedBy { get; set; }
    }
}
