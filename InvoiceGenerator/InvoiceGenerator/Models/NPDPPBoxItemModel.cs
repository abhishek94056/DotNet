// Models/NPDPPBoxItemModel.cs
namespace InvoiceGenerator.Models
{
    public class NPDPPBoxItemModel
    {
        public int NPD_ItemId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = "";
        public string Date { get; set; } = "";
        public string Marketing_Person { get; set; } = "";
        public string Item_Name { get; set; } = "";
        public string Item_Code { get; set; } = "";
        public string Customer_Name { get; set; } = "";
        public string Customer_Contact_Person { get; set; } = "";
        public string Customer_Contact_Details { get; set; } = "";
        public string Component_Name { get; set; } = "";
        public string Specifications { get; set; } = "";

        // ── PP Box Specific ──
        public string GSM_of_Box { get; set; } = "";
        public string GSM_of_Partition { get; set; } = "";
        public string Color_of_Box { get; set; } = "";
        public string Production_Person { get; set; } = "";
        public string Sheet_Size { get; set; } = "";
        public string Sheet_Size_Partition { get; set; } = "";
        public string Material_Color { get; set; } = "";
        public string Flap { get; set; } = "";
        public string Support_Party { get; set; } = "";
        public string Handle_Material_Grade { get; set; } = "";
        public string Handle_Fixing_Method { get; set; } = "";
        public string Cloth { get; set; } = "";

        // ── Documents ──
        public string Document_File_Name1 { get; set; } = "";
        public string Document_File_Name2 { get; set; } = "";
        public string Document_File_Name3 { get; set; } = "";
        public string Document_File_Name4 { get; set; } = "";
        public string Document_File_Name5 { get; set; } = "";

        // ── Quality & Delivery ──
        public string Printing_Matter { get; set; } = "";
        public string Cutting { get; set; } = "";
        public string Packing_Details { get; set; } = "";
        public string Delivery_Location { get; set; } = "";
        public string Quality_Person { get; set; } = "";
        public string SIR_DateTime { get; set; } = "";
        public string SIR_Remark { get; set; } = "";
        public string Transport_Delivery_Terms { get; set; } = "";
        public string Payment_Terms { get; set; } = "";
        public decimal Rate_of_Product { get; set; }
        public string Rework_Complaint_Details { get; set; } = "";

        public int IsActive { get; set; }
        public string Date_Time { get; set; } = "";
        public string CreatedBy { get; set; } = "";
    }
}