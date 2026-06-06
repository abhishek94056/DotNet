// Models/ItemSizeModel.cs
using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    public class ItemSizeModel
    {
        public int SizeId { get; set; }
        public int DepartmentId { get; set; }
        public string? Department { get; set; }
        public int? IsActive { get; set; }
        public string? ItemSize_Code { get; set; } 
        public string? Item_Size { get; set; } 
        public decimal Rate { get; set; }
        //public DateTime? Date { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; } 
        public decimal MIN_Stock { get; set; }
        public decimal MAX_Stock { get; set; }
    }

    // Department dropdown model
    public class DepartmentModel
    {
        public int DepartmentId { get; set; }
        public string Department { get; set; } = "";
    }
}