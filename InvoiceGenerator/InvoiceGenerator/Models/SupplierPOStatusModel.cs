// Models/SupplierPOStatusModel.cs
namespace InvoiceGenerator.Models
{
    public class SupplierPOStatusModel
    {
        // ── Core ──
        public int SrNo { get; set; }
        public int DepartmentId { get; set; }
        public int SupplierId { get; set; }
        public string PO_Number { get; set; } = "";
        public string PO_Date { get; set; } = "";
        public string Date_of_Requirement { get; set; } = "";
        public int SizeId { get; set; }
        public decimal PO_Rate { get; set; }
        public string Supplier_Invoice { get; set; } = "";
        public string Batch_Number { get; set; } = "";

        // ── RM Movement ──
        public int RM_Flag { get; set; }  // 1-5
        public decimal RM_Qty { get; set; }
        public string Date_of_Receipt { get; set; } = "";

        // ── Quality / Logistics ──
        public string Remarks { get; set; } = "";
        public decimal Debit_Amount_to_Supplier { get; set; }
        public int Supplier_Rating_Quality { get; set; }
        public int Supplier_Rating_Time { get; set; }
        public int Supplier_Rating_Process { get; set; }
        public string Rack_No_Storage_Location { get; set; } = "";
        public string Issue_with_Material { get; set; } = "";
        public string CAPA_Action_Plan_recd_from_Supplier { get; set; } = "";
        public string Action_Plan_Apporved_by_AE_Quality_Dept { get; set; } = "";
        public string Action_Plan_for_Supplier { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public int MonthId { get; set; }
        public int YearId { get; set; }

        // ── Display (from JOINs) ──
        public string Department { get; set; } = "";
        public string Supplier { get; set; } = "";
        public string ItemSize_Code { get; set; } = "";
        public string Item_Size { get; set; } = "";
        public decimal PO_Quantity { get; set; }
        public decimal RM_Receipt_Qty { get; set; }
        public decimal RM_Pro_Issue_Qty { get; set; }
        public decimal RM_Pro_Return_Qty { get; set; }
        public decimal RM_Supp_Return_Qty { get; set; }
        public decimal RM_FG_Qty { get; set; }
        public string Date_of_Supp_Return { get; set; } = "";
        public string Entry_Date { get; set; } = "";

        // ── For SelectAll_Month_Reporting ──
        public int Days_Material { get; set; }
    }
}