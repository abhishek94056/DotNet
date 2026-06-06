using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    public class CompanyModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public int StateCode { get; set; }
        public string State { get; set; }
        public string GSTIN { get; set; }
        public string PaymentTerm { get; set; }
    }
}
