using InvoiceGenerator.Models;

namespace InvoiceGenerator.Interfaces
{
    public interface IInvoiceService
    {
        int GetNextInvoiceNo();

        // Company
        List<CompanyModel> GetCompanies();
        CompanyModel? GetCompanyDetails(string name);

        // Items (Dropdown)
        List<ItemModel> GetAllItems();
        ItemModel? GetItemByName(string name);

        // Transport
        List<string> GetTransportModes();

        // Save Invoice
        int SaveInvoice(InvoiceModel master, List<ItemModel> items);

        // PDF
        (InvoiceModel? master, List<ItemModel> items) GetInvoiceForPdf(int invoiceNo);
    }
}