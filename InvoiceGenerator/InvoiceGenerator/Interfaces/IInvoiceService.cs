using InvoiceGenerator.Models;

namespace InvoiceGenerator.Interfaces
{
    public interface IInvoiceService
    {
        int GetNextInvoiceNo();

        List<CompanyDetails> GetCompanies();
        CompanyDetails? GetCompanyDetails(string name);

        List<InvoiceItem> GetAllItems();
        InvoiceItem? GetItemByName(string name);

        List<string> GetTransportModes();

        int SaveInvoice(InvoiceModel master, List<InvoiceItem> items);

        (InvoiceModel? master, List<InvoiceItem> items) GetInvoiceForPdf(int invoiceNo);
    }
}
