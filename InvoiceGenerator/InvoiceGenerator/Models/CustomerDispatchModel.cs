// Models/CustomerDispatchModel.cs
namespace InvoiceGenerator.Models
{
    public class CustomerDispatchModel
    {
        // ── Editable ──
        public int SrNo { get; set; }
        public int CustomerId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string Date { get; set; } = "";
        public int DepartmentId { get; set; }
        public string GRN_Status { get; set; } = "NO";
        public string CreatedBy { get; set; } = "";
        public int IsActive { get; set; }

        // ── Display (from JOINs) ──
        public string Customer { get; set; } = "";
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public string Department { get; set; } = "";
        public string ShiftName { get; set; } = "";
        public string Add_Date { get; set; } = "";
        public string GRN_Date { get; set; } = "";
        public string Invocie_No { get; set; } = "";
    }
}