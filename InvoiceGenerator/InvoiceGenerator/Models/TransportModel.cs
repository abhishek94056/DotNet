// Models/TransportMaster.cs
using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    public class TransportModel
    {
        public int Id { get; set; }
        public string ModeName { get; set; }
    }
}