using InvoiceGenerator.Models;

namespace InvoiceGenerator.Interfaces
{
    public interface ICompanyService
    {
        List<CompanyModel> GetAll();
        CompanyModel GetById(int id);
        int Insert(CompanyModel c);
        void Update(CompanyModel c);
        void Delete(int id);
    }
}
