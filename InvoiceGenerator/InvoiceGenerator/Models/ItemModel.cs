// Models/ItemMaster.cs
using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    public class ItemModel
    {
        public int ItemId { get; set; }
        public int SrNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string HSNCode { get; set; }
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal GST { get; set; }
        public decimal TaxableAmount { get; set; }  //(Rate × Qty)
        public decimal GSTAmount { get; set; }      //(TaxableAmount × GST%)
        public decimal Amount { get; set; }
    }
}