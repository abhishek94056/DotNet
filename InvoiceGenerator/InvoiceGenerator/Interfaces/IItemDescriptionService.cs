using InvoiceGenerator.Models;
using System.Collections.Generic;

namespace InvoiceGenerator.Interfaces
{
    public interface IItemDescriptionService
    {
        List<ItemDescriptionModel> GetAll();
        ItemDescriptionModel? GetById(int itemId);
        int Save(ItemDescriptionModel model);
        bool Delete(int itemId);
    }
}