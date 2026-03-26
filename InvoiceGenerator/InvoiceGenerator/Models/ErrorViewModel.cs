namespace InvoiceGenerator.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }    //string? → nullable (can be null)

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
