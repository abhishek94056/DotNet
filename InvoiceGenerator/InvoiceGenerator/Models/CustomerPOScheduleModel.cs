// Models/CustomerPOScheduleModel.cs
namespace InvoiceGenerator.Models
{
    public class CustomerPOScheduleModel
    {
        // ── Editable ──
        public int SrNo { get; set; }
        public int CustomerId { get; set; }
        public string PO_Number { get; set; } = "";
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal PO_Item_Rate { get; set; }
        public string PO_Date { get; set; } = "";
        public string Delivery_Date { get; set; } = "";
        public int DepartmentId { get; set; }
        public string CreatedBy { get; set; } = "";

        // ── Display (from JOINs) ──
        public string Customer { get; set; } = "";
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string Date { get; set; } = "";
        public string Plan_Date { get; set; } = "";

        public int IsActive { get; set; }
    }

}