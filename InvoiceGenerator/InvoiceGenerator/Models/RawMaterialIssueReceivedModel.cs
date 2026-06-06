//// Models/RawMaterialIssueReceivedModel.cs
//namespace InvoiceGenerator.Models
//{
//    public class RawMaterialIssueReceivedModel
//    {
//        // Table Fields
//        public int SrNo { get; set; }
//        public int DepartmentId { get; set; }
//        public int SizeId { get; set; }

//        public decimal Issue_Quantity { get; set; }
//        public decimal Received_Quantity { get; set; }

//        public int IsActive { get; set; }

//        public string Date { get; set; } = "";
//        public string CreatedBy { get; set; } = "";

//        // Display Fields (From JOINs)
//        public string ItemSize_Code { get; set; } = "";
//        public string Item_Size { get; set; } = "";
//        public string Department { get; set; } = "";
//        public string Add_Date { get; set; } = "";

//        // Extra Fields Used in SP Parameters
//        public decimal Quantity { get; set; }
//        public int RM_Status { get; set; }
//    }
//}

// Models/RawMaterialIssueModel.cs
namespace InvoiceGenerator.Models
{
    public class RawMaterialIssueReceivedModel
    {
        // ── Editable ──
        public int SrNo { get; set; }
        public int DepartmentId { get; set; }
        public int SizeId { get; set; }
        public decimal Quantity { get; set; }
        public string Date { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public int RM_Status { get; set; }  // 1 = Issue, 2 = Received
        public int IsActive { get; set; }

        // ── Display (from JOINs) ──
        public string ItemSize_Code { get; set; } = "";
        public string Item_Size { get; set; } = "";
        public string Department { get; set; } = "";
        public string Add_Date { get; set; } = "";
        public decimal Issue_Quantity { get; set; }
        public decimal Received_Quantity { get; set; }

        // ── Computed ──
        public decimal Balance =>
            Received_Quantity - Issue_Quantity;
    }
}