// Models/ManualScheduleDispatchModel.cs
namespace InvoiceGenerator.Models
{
    public class ManualScheduleDispatchModel
    {
        // ── Editable ──
        public int SrNo { get; set; }
        public int DepartmentId { get; set; }
        public int CustomerId { get; set; }
        public int ItemId { get; set; }
        public int Tentative_PO_Qty { get; set; }
        public int Req_Quantity { get; set; }
        public int Dis_Quantity { get; set; }
        public string Date { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public int IsActive { get; set; }

        // ── Display (from JOINs) ──
        public string Department { get; set; } = "";
        public string Customer { get; set; } = "";
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public string System_Date { get; set; } = "";
    }

    // ── Report model (SelectAll_Report — day-wise pivot) ──
    public class ManualScheduleReportModel
    {
        public int DepartmentId { get; set; }
        public string Department { get; set; } = "";
        public int CustomerId { get; set; }
        public string Customer { get; set; } = "";
        public int ItemId { get; set; }
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public int? MIN_Stock { get; set; }
        public int? MAX_Stock { get; set; }

        // ── Month totals ──
        public int Month_Total_Req_Qty { get; set; }
        public int Month_Total_Dis_Qty { get; set; }
        public int Tentative_PO_Qty { get; set; }

        // ── Day-wise Req + Dis (Day 1 to 31) ──
        public int[] DayReq { get; set; } = new int[32];
        public int[] DayDis { get; set; } = new int[32];
    }
}