// Models/ControlPlanModel.cs
namespace InvoiceGenerator.Models
{
    public class ControlPlanModel
    {
        // ── Editable ──
        public int SrNo { get; set; }
        public int DepartmentId { get; set; }
        public int ItemId { get; set; }
        public string Pressure_Time { get; set; } = "";
        public string Punching_Pressure { get; set; } = "";
        public string Set_mm { get; set; } = "";
        public string Cycle_Delay_Time { get; set; } = "";
        public string Vaccum_Time { get; set; } = "";
        public string Cooling_Time { get; set; } = "";
        public string Ejection_Time { get; set; } = "";
        public string Winder_Time { get; set; } = "";
        public string Zone_1 { get; set; } = "";
        public string Zone_2 { get; set; } = "";
        public string Zone_3 { get; set; } = "";
        public string Zone_4 { get; set; } = "";
        public string Zone_5 { get; set; } = "";
        public string Zone_6 { get; set; } = "";
        public string Zone_7 { get; set; } = "";
        public string Zone_8 { get; set; } = "";
        public string Zone_9 { get; set; } = "";
        public string Zone_10 { get; set; } = "";
        public string Zone_11 { get; set; } = "";
        public string Zone_12 { get; set; } = "";
        public string Zone_13 { get; set; } = "";
        public string Zone_14 { get; set; } = "";
        public string Zone_15 { get; set; } = "";
        public string Zone_16 { get; set; } = "";
        public string Packing_Details { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public int IsActive { get; set; }

        // ── Display ──
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public string Department { get; set; } = "";
        public string Add_Date { get; set; } = "";
    }
}