// Models/ItemMaster.cs
using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    public class ItemModel
    {
        public int ItemId { get; set; }
        public int SrNo { get; set; }

        [Required(ErrorMessage = "ItemCode is required")]
        public string ItemCode { get; set; }

        [Required(ErrorMessage = "Item Name is required")]
        [StringLength(100, ErrorMessage = " cannot exceed 100 characters")]
        public string ItemDescription { get; set; }

        [Required(ErrorMessage = "HSN Code is required")]
        [StringLength(50, ErrorMessage = "HSNCode cannot exceed 50 characters")]
        public string HSNCode { get; set; }

        [Required(ErrorMessage = "Rate is required")]
        [Range(0.01, 9999999.99, ErrorMessage = "Rate must be greater than 0")]
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        [Required(ErrorMessage = "GST is required")]
        public decimal GST { get; set; }
        public decimal TaxableAmount { get; set; }  //(Rate × Qty)
        public decimal GSTAmount { get; set; }      //(TaxableAmount × GST%)
        public decimal Amount { get; set; }
    }
}