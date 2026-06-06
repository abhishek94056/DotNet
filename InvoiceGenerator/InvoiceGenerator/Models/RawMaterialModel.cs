// Models/RawMaterialModel.cs
namespace InvoiceGenerator.Models
{
    public class RawMaterialModel
    {
        // ── Editable fields ──
        public int SrNo { get; set; }
        public int SizeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal RM_Rate { get; set; }
        public int DepartmentId { get; set; }
        public string CreatedBy { get; set; } = "";

        // ── Display-only fields (from JOINs) ──
        public string ItemSize_Code { get; set; } = "";
        public string Item_Size { get; set; } = "";
        public string Add_Date { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public decimal Total_Value { get; set; }

        // ── View-only fields (stored in DB, not editable in UI) ──
        public int SupplierId { get; set; }
        public string Supplier { get; set; } = "";
        public string PO_Number { get; set; } = "";
        public string Invoice_No { get; set; } = "";
        public string Batch_Number { get; set; } = "";

        public int IsActive { get; set; }
    }
}