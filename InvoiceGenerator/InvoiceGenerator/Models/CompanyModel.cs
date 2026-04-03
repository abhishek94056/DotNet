using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    public class CompanyModel
    {
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(100, ErrorMessage = " cannot exceed 100 characters")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "State Code is required")]
        public int StateCode { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        public string State { get; set; }

        [Required(ErrorMessage = "GSTIN is required")]
        [StringLength(20, ErrorMessage = "GSTIN cannot exceed 50 characters")]
        public string GSTIN { get; set; }

        [Required(ErrorMessage = "Payment Term is required")]
        [StringLength(50, ErrorMessage = "PaymentTerm cannot exceed 50 characters")]
        public string PaymentTerm { get; set; }
    }
}
