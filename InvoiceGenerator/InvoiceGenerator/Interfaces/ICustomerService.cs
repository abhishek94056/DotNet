using InvoiceGenerator.Models;

namespace InvoiceGenerator.Interfaces
{
    public interface ICustomerService
    {
        List<CustomerModel> GetAll();
        CustomerModel GetById(int customerId);
        int Insert(CustomerModel c);
        void Update(CustomerModel c);
        void Delete(int customerId);
    }
}
