using InvoiceGenerator.Models;

namespace InvoiceGenerator.Interfaces
{
    public interface ITransportService
    {
        List<TransportModel> GetAll();
        TransportModel? GetById(int id);
        int Insert(TransportModel t);
        void Update(TransportModel t);
        void Delete(int id);
    }
}
