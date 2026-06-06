// Models/MatrixItemSizeModel.cs
namespace InvoiceGenerator.Models
{
    public class MatrixItemSizeModel
    {
        public int MatrixId { get; set; }
        public int SizeId { get; set; }
        public string ItemSize_Code { get; set; } = "";
        public string Item_Size { get; set; } = "";
        public int ItemId { get; set; }
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public int DepartmentId { get; set; }
        public string Department { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "";
    }

    // For raw form binding
    public class MatrixItemSizeFormModel
    {
        public int MatrixId { get; set; }
        public int SizeId { get; set; }
        public int ItemId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "";
    }
}