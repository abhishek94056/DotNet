namespace InvoiceGenerator.Models
{
    public class MatrixItemCustomerModel
    {
        public int MatrixId { get; set; }
        public int CustomerId { get; set; }
        public string Customer { get; set; } = "";
        public int ItemId { get; set; }
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "";
    }

    public class MatrixItemCustomerFormModel
    {
        public int MatrixId { get; set; }
        public int CustomerId { get; set; }
        public int ItemId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "";
    }
}