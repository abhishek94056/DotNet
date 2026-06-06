using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    // Models/InvoiceMaster.cs
    public class InvoiceModel
    {
        public int InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DateOfSupply { get; set; }
        public string PurchaseOrderNo { get; set; }
        public DateTime PurchaseOrderDt { get; set; }
        public string VehicleNo { get; set; }
        public string ASNNo { get; set; }
        public string InvoiceTo { get; set; }
        public string ShippingTo { get; set; }
        public decimal TaxableValue { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal TotalValue { get; set; }
        public string TransportMode { get; set; }
        public string Remark { get; set; }
    }
}
