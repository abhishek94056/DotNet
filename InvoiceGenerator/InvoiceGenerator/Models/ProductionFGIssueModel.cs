// Models/ProductionFGIssueModel.cs
namespace InvoiceGenerator.Models
{
    public class ProductionFGIssueModel
    {
        // ── Editable ──
        public int SrNo { get; set; }
        public int DepartmentId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string Date { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public int IsActive { get; set; }

        // ── Display (from JOINs) ──
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public string Department { get; set; } = "";
        public string Add_Date { get; set; } = "";
    }
}