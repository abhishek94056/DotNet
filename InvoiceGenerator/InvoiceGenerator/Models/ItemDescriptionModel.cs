using System;

namespace InvoiceGenerator.Models
{
    public class ItemDescriptionModel
    {
        public int ItemId { get; set; }

        public string? Item_Code { get; set; }
        public string? Item_Description { get; set; }

        public int Cycle_Time { get; set; }
        public int No_of_Cavity { get; set; }

        public decimal Std_Shot_Weight { get; set; }
        public decimal Finish_Weight { get; set; }

        public decimal MRP_Rate_RM { get; set; }
        public decimal MRP_Rate_Sale { get; set; }

        public string? Ext_Mould_PVC_RMType { get; set; }

        public int DepartmentId { get; set; }
        public string? Department { get; set; }

        public int? MachineId { get; set; }
        public string? MachineName { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }

        public int MIN_Stock { get; set; }
        public int MAX_Stock { get; set; }
        public int Opening_Stock { get; set; }

        public int? RM_SizeId_1 { get; set; }
        public int Sheet_Qty_1 { get; set; }
        public int? RM_SizeId_2 { get; set; }
        public int Sheet_Qty_2 { get; set; }
        public int? RM_SizeId_3 { get; set; }
        public int Sheet_Qty_3 { get; set; }
        public int? RM_SizeId_4 { get; set; }
        public int Sheet_Qty_4 { get; set; }

        public string? SizeName_1 { get; set; }
        public string? SizeName_2 { get; set; }
        public string? SizeName_3 { get; set; }
        public string? SizeName_4 { get; set; }
        public string? ItemSize_Code_1 { get; set; }
        public string? ItemSize_Code_2 { get; set; }
        public string? ItemSize_Code_3 { get; set; }
        public string? ItemSize_Code_4 { get; set; }

        // ✅ PACKING (CORRECT)
        public int? PackingId { get; set; }
        public string? Packing_Type { get; set; }
        public int Packing_Qty { get; set; }

        // ✅ INNER PACKING (USED IN OTHER VIEW)
        public int? Inner_PackingId { get; set; }
        public string? Inner_Packing_Type { get; set; }
        public int Inner_Packing_Qty { get; set; }

        public int Lab { get; set; }

        public string? Remark_Std_Shot_Weight { get; set; }
        public string? Remark_Finish_Weight { get; set; }
        public string? Remark_MRP_Rate_RM { get; set; }
        public string? Remark_MRP_Rate_Sale { get; set; }

        public decimal Sale_Trans_Cost { get; set; }
    }
}