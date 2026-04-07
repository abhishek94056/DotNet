using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    // Models/InvoiceMaster.cs
    public class InvoiceModel
    {
        public int InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DateOfSupply { get; set; }
        [StringLength(100, ErrorMessage = "PurchaseOrderNo cannot exceed 50 characters")]
        public string PurchaseOrderNo { get; set; }
        public DateTime PurchaseOrderDt { get; set; }
        [StringLength(100, ErrorMessage = "VehicleNo cannot exceed 250 characters")]
        public string VehicleNo { get; set; }
        [StringLength(100, ErrorMessage = "ASNNo cannot exceed 50 characters")]
        public string ASNNo { get; set; }
        public string InvoiceTo { get; set; }
        public string ShippingTo { get; set; }
        public decimal TaxableValue { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal TotalValue { get; set; }
        public string TransportMode { get; set; }
        [StringLength(100, ErrorMessage = "Remark cannot exceed 250 characters")]
        public string Remark { get; set; }
    }
}
