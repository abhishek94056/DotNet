using InvoiceGenerator.Models;

namespace InvoiceGenerator.Interfaces
{
    public interface IItemService
    {
        List<ItemModel> GetAll();
        ItemModel? GetById(int id);
        int Insert(ItemModel item);
        void Update(ItemModel item);
        void Delete(int id);
    }
}
